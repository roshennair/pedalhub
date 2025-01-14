﻿using Microsoft.AspNetCore.Mvc;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Authorization;
using PedalHub.Data;
using PedalHub.Models;
using Microsoft.EntityFrameworkCore;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.AspNetCore.Identity;
using PedalHub.Areas.Identity.Data;

namespace PedalHub.Controllers
{
    public class BikesController : Controller
    {
        private readonly PedalHubContext _context;
        private readonly UserManager<PedalHubUser> _userManager;

        public BikesController(PedalHubContext context, UserManager<PedalHubUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private const string s3BucketName = "pedalhub-bike-images";
        private const string cloudfrontDomain = "d35soteeq6wtnk.cloudfront.net";
        private const string snsTopicARN = "arn:aws:sns:us-east-1:688301327840:PedalHub";

        private List<string> getKeys()
        {
            List<string> keys = new List<string>();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();

            keys.Add(configuration["Values:Key1"]);
            keys.Add(configuration["Values:Key2"]);
            keys.Add(configuration["Values:Key3"]);

            return keys;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.User.IsInRole("Administrator"))
            {
                return RedirectToAction("AdminIndex");
            }

            List<Bike> bikeList = await _context.Bike.ToListAsync();
            return View(bikeList);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AdminIndex()
        {
            List<Bike> bikeList = await _context.Bike.ToListAsync();
            return View(bikeList);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult AddBike()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Bike bike, IFormFile imageFile)
        {
            var imageKey = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

            List<string> keys = getKeys();
            var awsS3Client = new AmazonS3Client(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);

            try
            { 
                PutObjectRequest uploadRequest = new PutObjectRequest
                {
                    InputStream = imageFile.OpenReadStream(),
                    BucketName = s3BucketName,
                    Key = "images/" + imageKey,
                    CannedACL = S3CannedACL.PublicRead
                };

                await awsS3Client.PutObjectAsync(uploadRequest);
            }
            catch (AmazonS3Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }

            bike.ImageURL = "https://" + cloudfrontDomain + "/images/" + imageKey;
            bike.ImageKey = imageKey;

            _context.Bike.Add(bike);
            await _context.SaveChangesAsync();

            // send email broadcast
            var broadcastText = "Hello dear PedalHub users! We have just added a new bike type to our collection called " + bike.Name + ". Head over to our website to reserve one for yourself now!";
            var snsClient = new AmazonSimpleNotificationServiceClient(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);
            try
            {
                PublishRequest publishRequest = new PublishRequest(snsTopicARN, broadcastText);
                PublishResponse publishResponse = await snsClient.PublishAsync(publishRequest);
            }
            catch (AmazonSimpleNotificationServiceException ex)
            {
                return BadRequest(ex.Message);
            }

            return RedirectToAction("Index", "Bikes");
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> EditBike(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Bike bike = await _context.Bike.FindAsync(id);

            if (bike == null)
            {
                return BadRequest("Bike with ID " + id + " is not found.");
            }

            return View(bike);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(Bike updatedBike)
        {
            if (updatedBike == null)
            {
                return NotFound();
            }

            _context.Bike.Update(updatedBike);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Bikes");
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> ReserveBike(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Bike bike = await _context.Bike.FindAsync(id);

            if (bike == null)
            {
                return BadRequest("Bike with ID " + id + " is not found.");
            }

            return View(bike);
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Reserve(int? bikeId, string? userId)
        {
            if (bikeId == null || userId == null) 
            {
                return NotFound();    
            }

            Bike bike = await _context.Bike.FindAsync(bikeId);

            if (bike == null)
            {
                return BadRequest("Bike with ID " + bikeId + " is not found.");
            }

            Reservation newReservation = new Reservation();
            newReservation.UserId = userId;
            newReservation.BikeId = (int)bikeId;
            newReservation.CreatedAt = DateTime.Now;

            _context.Reservation.Add(newReservation);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Bikes");
        }
    }
}
