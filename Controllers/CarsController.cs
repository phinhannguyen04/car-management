using CarManagement.Data;
using CarManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarManagement.Controllers
{
    public class CarsController : Controller
    {
        private readonly CarRepository _carRepository;

        public CarsController (CarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            var pageSize = 10; // giới hạn số bản ghi mỗi trang
            var cars = await _carRepository.GetCarsAsync(page, pageSize); // lấy các bản ghi tại page=1 và mỗi trang có 10 bản ghi

            
            var totalCars = await _carRepository.GetTotalCarsAsync(); // lấy tổng số bản ghi để tính số phân trang
            var highestPriceCar = await _carRepository.GetHighestPriceCarAsync(); // lấy về đối tượng có giá cao nhất
            var totalPages = (int)Math.Ceiling((double)totalCars / pageSize);

            // Truyền dữ liệu vào View
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.HighestPriceCar = highestPriceCar;

            return View(cars);
        }

        [HttpPost]
        public async Task<IActionResult> Create (Car car)
        {
            await _carRepository.CreateAsync(car);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit (Car car)
        {
            await _carRepository.UpdateCarAsync(car);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete (string id)
        {
            var car = new Car { id = id };
            await _carRepository.DeleteCarAsync(car);
            return RedirectToAction("Index");
        }
    }
}
