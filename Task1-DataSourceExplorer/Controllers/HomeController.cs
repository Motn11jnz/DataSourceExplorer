using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NuGet.Protocol.Plugins;
using Task1_DataSourceExplorer.Models;
using Task1_DataSourceExplorer.Repository;

namespace Task1_DataSourceExplorer.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private DataExplorerRepo _dataExplorerRepo;
        private string _connection = "";

        public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
            _dataExplorerRepo = new DataExplorerRepo();
			

        }

		public IActionResult Index()
		{
            if (TempData.ContainsKey("ErrorMessage"))
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }
            return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult SqlConnection(ConnectionViewModel connection)
		{
			(string connectionMsg,string conString) = _dataExplorerRepo.SqlConnectionRepo(connection);

			if (string.IsNullOrEmpty(connectionMsg))
			{
				HttpContext.Session.SetString("_connection", conString);
				return RedirectToAction("ListTablesView", "Home", connection);
            }

            TempData["ErrorMessage"] = $"Connection failed: {connectionMsg}";

            return RedirectToAction("Index", "Home");
        }

		public IActionResult ListTablesView()
		{
			_connection = HttpContext.Session.GetString("_connection");
            List<string> tablesLs = _dataExplorerRepo.getSqlTablesRepo(_connection);

			ListTablesViewModel model = new ListTablesViewModel
			{
				TableNames = tablesLs,
				SelectedTableName = null,
                TableColumnViewModel = null
            };

			ExecuteQueryModel executeQueryModel = new ExecuteQueryModel
			{
				ListTablesViewModel = model,
				dataTable = null
			};

            return View(executeQueryModel);
        }

        [HttpPost]
        public async Task<ActionResult> ListTablesView(ListTablesViewModel model)
        {
            _connection = HttpContext.Session.GetString("_connection");

            List<string> columnNames = await _dataExplorerRepo.getSqlColumnsRepo(_connection, model.SelectedTableName);

            ExecuteQueryModel executeQueryModel = new ExecuteQueryModel
            {
                ListTablesViewModel = new ListTablesViewModel
                {
                    TableColumnViewModel = new TableColumnViewModel
                    {
                        TableName = model.SelectedTableName,
                        ColumnNames = columnNames
                    },
                    TableNames = model.TableNames,
                    SelectedTableName = model.SelectedTableName
                },
                dataTable = null
            };


            return View("ListTablesView", executeQueryModel);
        }

        [HttpPost]
        public async Task<ActionResult> QueryList(string ListTablesViewModel, string Query)
        {
            _connection = HttpContext.Session.GetString("_connection");

            DataTable dataTable = await _dataExplorerRepo.ExecuteQuery(_connection, Query);

            ListTablesViewModel model = JsonSerializer.Deserialize<ListTablesViewModel>(ListTablesViewModel);


            ExecuteQueryModel executeQueryModel = new ExecuteQueryModel
            {
                ListTablesViewModel = new ListTablesViewModel
                {
                    TableColumnViewModel = new TableColumnViewModel
                    {
                        TableName = model.SelectedTableName,
                        ColumnNames = model.TableColumnViewModel.ColumnNames
                    },
                    TableNames = model.TableNames,
                    SelectedTableName = model.SelectedTableName
                },
                dataTable = dataTable
            };

            return View("ListTablesView", executeQueryModel);
        }

    }
}