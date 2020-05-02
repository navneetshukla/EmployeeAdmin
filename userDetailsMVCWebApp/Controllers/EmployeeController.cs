using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using userDetailsMVCWebApp.Models;

namespace userDetailsMVCWebApp.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeContext _context;
        public readonly IConfiguration _configuration;
        public string filename = string.Empty;
        
        static CloudBlobClient _blobClient;
        const string blobContainerName = "imagecontainer";
        static CloudBlobContainer _blobContainer;
        
        public EmployeeController(EmployeeContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;               
        }

        // GET: Employee
        public async Task<IActionResult> Index()
        {                                    
            return View(await _context.Employees.ToListAsync());           
        }

        // GET: Employee/Create
        public async Task<IActionResult> AddOrEdit(int id = 0, string? FileName = "", bool? isDeleted = false) { 
            if (FileName != "")
            {   
                var blob = GetBlob(FileName);
                if (blob.GetType() == typeof(CloudBlockBlob))
                    ViewData["image"] = blob.Uri;
            }

            if (id == 0)
                return View(new Employee());
            else
            {
                var emp = _context.Employees.Find(id);
                if (isDeleted == false)
                {
                    var blob = GetBlob(emp.fileName);
                    if (blob.GetType() == typeof(CloudBlockBlob))
                        ViewData["image"] = blob.Uri;                    
                }
                return View(emp);
            }
        }

        // POST: Employee/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("EmployeeId,EmployeeName,EmailId,DOB,Gender,MarritalStatus,Anniversary,fileName")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                if (employee.EmployeeId == 0)
                    _context.Add(employee);
                else
                    _context.Update(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }            
            return View(employee);
        }
       
        // GET: Employee/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
           
        }

       
        private string GetRandomBlobName(string filename)
        {
            string ext = Path.GetExtension(filename);
            return string.Format("{0:10}_{1}{2}", DateTime.Now.Ticks, Guid.NewGuid(), ext);
        }


        public CloudBlockBlob GetBlob(string imageName)
        {
            var storageConnectionSring = _configuration.GetValue<string>("StorageConnectionString");
            var storageAccount = CloudStorageAccount.Parse(storageConnectionSring);
            _blobClient = storageAccount.CreateCloudBlobClient();
            _blobContainer = _blobClient.GetContainerReference(blobContainerName);
            Uri uri = new Uri(imageName);
            string filename = Path.GetFileName(uri.LocalPath);
            var blob = _blobContainer.GetBlockBlobReference(filename);
            return blob;            
        }

        [HttpPost]
        public async Task<ActionResult> UploadFile(int employeeId= 0)
        {
            try
            {                
                var request = await HttpContext.Request.ReadFormAsync();
                if (request.Files == null)
                {
                    return BadRequest("No files or Could not read files");                    
                }                
                var files = request.Files;
                if (files.Count == 0)
                {
                    return BadRequest("Could not upload empty Files");
                }
                var storageConnectionSring = _configuration.GetValue<string>("StorageConnectionString");
                var storageAccount = CloudStorageAccount.Parse(storageConnectionSring);
                _blobClient = storageAccount.CreateCloudBlobClient();
                _blobContainer = _blobClient.GetContainerReference(blobContainerName);
                await _blobContainer.CreateIfNotExistsAsync();
                await _blobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                bool isDeleted = false;
                if (employeeId != 0)
                {
                    var emp = _context.Employees.Find(employeeId);
                    Uri uri = new Uri(emp.fileName);
                    string imageName = Path.GetFileName(uri.LocalPath);
                    var deleteBlob = _blobContainer.GetBlockBlobReference(imageName);
                    await deleteBlob.DeleteIfExistsAsync();
                    isDeleted = true;
                }
                var blob = _blobContainer.GetBlockBlobReference(GetRandomBlobName(files[0].FileName));
                using (var stream = files[0].OpenReadStream())
                {
                    await blob.UploadFromStreamAsync(stream);
                }
                return RedirectToAction("AddOrEdit", new {id = employeeId,  fileName = blob.Uri , isDeleted } );
            }
            catch (Exception ex)
            {
                ViewData["message"] = ex.Message;
                ViewData["trace"] = ex.StackTrace;
                return View("Error");
            }
        } 
        [HttpPost]
        public async Task<ActionResult> DeleteImage( string name, int id = 0)
        {
            try
            {
                Uri uri = new Uri(name);
                string filename = Path.GetFileName(uri.LocalPath);

                var blob = _blobContainer.GetBlockBlobReference(filename);
                await blob.DeleteIfExistsAsync();

                return RedirectToAction("AddOrEdit", new { id = id , isDeleted= true});
            }
            catch (Exception ex)
            {
                ViewData["message"] = ex.Message;
                ViewData["trace"] = ex.StackTrace;
                return View("Error");
            }
        }

       
    }
}
