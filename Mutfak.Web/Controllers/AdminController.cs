using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mutfak.Domain.Entity;
using Mutfak.Domain.Repo;
using Mutfak.Util;
using Mutfak.Web.App_Start;
using Mutfak.Web.Models;

namespace Mutfak.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IFormsAuthenticationService _formsAuthenticationService;
        private readonly IEntityRepository<Product> _productRepo;

        public AdminController(
            IFormsAuthenticationService formsAuthenticationService, IEntityRepository<Product> productRepo)
        {
            _formsAuthenticationService = formsAuthenticationService;
            _productRepo = productRepo;
        }

        [HttpGet]
        public async Task<ViewResult> Index(int page = 1)
        {
            return View(await MapProductModel(page));
        }

        private async Task<GridModel> MapProductModel(int page)
        {
            var model = new GridModel
            {
                Name = "Ürün",
                AddLink = "/admin/add",
                TableHeaders = new List<string> { "Kodu", "Kategorileri", "Başlık", "Adı", "Fiyatı", "Açıklama", "Resim" },
                Title = "Ürün Yönetim Ekranı",
                IsAddButtonVisible = true
            };

            model.Keyword = model.Name.ToUrlSlug();
            model.DataLinesAsHtmlTr = "<tr>";

            var items = _productRepo.AsOrderedQueryable().ToPaginatedList(page, 15);
            if (items != null
                && items.Items != null
                && items.Items.Any())
            {
                foreach (var dto in items.Items)
                {
                    var btnDel = string.Format("<input id='btnDel{0}' type='button' class='btn btn-mini btn-danger confirm-delete' data-id='{0}' data-name='{1}' value='Sil' />",
                                               dto.Id, dto.Name);

                    var btnPasive = string.Format(dto.IsSold ? "<input id='btnSoldCancel{0}' type='button' class='btn btn-mini btn-success btn-unsold' data-id='{0}' data-name='{1}' value='Satış İptal' />"
                                                             : "<input id='btnSold{0}' type='button' class='btn btn-mini btn-success btn-sold' data-id='{0}' data-name='{1}' value='Satıldı' />", dto.Id, dto.Name);


                    var btnEdit = string.Format("<input type=\"button\" class=\"btn btn-mini btn-info\" onclick=\"javascript:document.location.href='/admin/edit/{0}';\" value=\"Düzenle\" /><br/><br/>", dto.Id);

                    //var btnTweet = string.Format("<input type=\"button\" class=\"btn btn-mini btn-primary\" onclick=\"javascript:document.location.href='/admin/sendtweeter/{0}';\" value=\"Send Tweeter\" /><br/><br/>", dto.Id);
                    //var btnFacebook = string.Format("<input type=\"button\" class=\"btn btn-mini btn-primary\" onclick=\"javascript:document.location.href='/admin/sendfacebook/{0}';\" value=\"Send Facebook\" /><br/>", dto.Id);

                    model.DataLinesAsHtmlTr += string.Format("<tr><td>{0} {1} {2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td><a href='/product/{3}' target='_blank'>{6}</a></td><td>{7}</td><td>{8}</td><td><img src='{9}' alt='{5}' class='img-polaroid' style='' /></td></tr>",
                        btnDel, btnPasive, btnEdit, dto.Code, dto.Category, dto.Title, dto.Name, string.Format("{0:0,0}", dto.Price), dto.Description, dto.ImageUrlPrimary);
                }

                model.Paging = new PagingModel
                {
                    PageIndex = page,
                    TotalPageCount = items.TotalPageCount,
                    HasNextPage = items.HasNextPage,
                    HasPreviousPage = items.HasPreviousPage,
                    PageUrl = Request.Url != null ? Request.Url.AbsolutePath : string.Empty
                };
            }

            return await Task.FromResult(model);
        }

        [HttpGet, AllowAnonymous]
        public ViewResult Login()
        {
            return this.View();
        }

        [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var usercnfg = ConfigurationManager.AppSettings["User"];
                var passcnfg = ConfigurationManager.AppSettings["Pass"];

                if (model.User == usercnfg && model.Password == passcnfg)
                {
                    _formsAuthenticationService.SignIn(model.User, true);
                    return RedirectToAction("Index");
                }
            }

            model.Msg = "lütfen tekrar deneyiniz";
            return View(model);
        }

    }
}
