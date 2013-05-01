using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using TaggedProducts.Domain.Entity;
using TaggedProducts.Domain.Repo;
using TaggedProducts.Utils;
using TaggedProducts.Web.App_Start;
using TaggedProducts.Web.Models;

namespace TaggedProducts.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IFormsAuthenticationService _formsAuthenticationService;
        private readonly IEntityRepository<Product> _productRepo;
        private readonly IEntityRepository<Tag> _tagRepo;

        public AdminController(
            IFormsAuthenticationService formsAuthenticationService,
            IEntityRepository<Product> productRepo,
            IEntityRepository<Tag> tagRepo)
        {
            _formsAuthenticationService = formsAuthenticationService;
            _productRepo = productRepo;
            _tagRepo = tagRepo;
        }

        #region Product

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
                        btnDel, btnPasive, btnEdit, dto.Code, GetTagContainer(dto.Id), dto.Title, dto.Name, string.Format("{0:0,0}", dto.Price), dto.Description, dto.ImageUrlPrimary);
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

        [HttpPost]
        public JsonResult ProductDelete(string id)
        {
            var result = new BaseModel();

            ObjectId pId;
            if (!string.IsNullOrEmpty(id)
                && ObjectId.TryParse(id, out pId))
            {
                if (_productRepo.AsQueryable().Any(x => x.Id == pId))
                {
                    result.OK = _productRepo.Delete(new Product
                    {
                        DeletedBy = User.Identity.Name,
                        DeletedOn = DateTime.Now,
                        Id = pId
                    }).Ok;
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ProductImageDelete(string id, string prodId)
        {
            var result = new BaseModel();

            ObjectId pId;
            int imgOrder;
            if (!string.IsNullOrEmpty(id)
                && int.TryParse(id, out imgOrder)
                && ObjectId.TryParse(prodId, out pId))
            {
                var property = _productRepo.AsQueryable().FirstOrDefault(w => w.Id == pId);
                if (property != null && property.ImageUrls.Count > imgOrder)
                {
                    var imgUrls = property.ImageUrls;
                    imgUrls.RemoveAt(imgOrder);

                    result.OK = _productRepo.Update(Query<Product>.EQ(x => x.Id, pId),
                                                      Update<Product>.Set(x => x.ImageUrls, imgUrls)
                                                                      .Set(x => x.ImageUrlPrimary, imgUrls[0])).Ok;
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SetSold(string id)
        {
            var result = new BaseModel();

            ObjectId pId;
            if (!string.IsNullOrEmpty(id)
                && ObjectId.TryParse(id, out pId))
            {
                if (_productRepo.AsQueryable().Any(x => x.Id == pId))
                {
                    result.OK = _productRepo.Update(Query<Product>.EQ(x => x.Id, pId),
                                             Update<Product>.Set(x => x.IsSold, true)
                                                            .Set(x => x.UpdatedOn, DateTime.Now)
                                                            .Set(x => x.UpdatedBy, User.Identity.Name)).Ok;
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SetUnSold(string id)
        {
            var result = new BaseModel();

            ObjectId pId;
            if (!string.IsNullOrEmpty(id)
                && ObjectId.TryParse(id, out pId))
            {
                if (_productRepo.AsQueryable().Any(x => x.Id == pId))
                {
                    result.OK = _productRepo.Update(Query<Product>.EQ(x => x.Id, pId), 
                                             Update<Product>.Set(x => x.IsSold, false)
                                                            .Set(x => x.UpdatedOn, DateTime.Now)
                                                            .Set(x => x.UpdatedBy, User.Identity.Name)).Ok;
                }
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        private static DataTable ConvertToDataTable<T>(IEnumerable<T> data)
        {
            var properties = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            foreach (var item in data)
            {
                var row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;

                table.Rows.Add(row);
            }

            return table;
        }

        [HttpGet]
        public ActionResult ExportExcel()
        {
            var sb = new StringBuilder();
            sb.Append("<table border='" + "2px" + "'b>");
            //write column headings
            sb.Append("<tr>");
            var dt = ConvertToDataTable(_productRepo.AsQueryable().Select(x => new
            {
                x.Code,
                x.Name,
                x.Description,
                x.Title,
                x.Price,
                x.Currency,
                x.ImageUrlPrimary,
                x.ImageUrlPrimarySmall,
                x.ImageUrlPrimaryBig,
                x.ImageUrls,
                x.ImageUrlBigs,
                x.ImageUrlSmalls,
                x.VideoUrl,
                x.Tags,
                x.IsSold
            }).ToList());
            foreach (var dc in dt.Columns)
            {
                sb.Append("<td><b><font face=Arial size=2>" + dc + "</font></b></td>");
            }
            sb.Append("</tr>");

            //write table data
            foreach (DataRow dr in dt.Rows)
            {
                sb.Append("<tr>");
                foreach (DataColumn dc in dt.Columns)
                {
                    sb.Append("<td><font face=Arial size=" + "14px" + ">" + dr[dc] + "</font></td>");
                }
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            Response.AddHeader("Content-Disposition", "Products.xls");
            Response.ContentType = "application/vnd.ms-excel";
            Response.ContentEncoding = Encoding.UTF8;
            const string header = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\n<head>\n<title></title>\n<meta http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1254\" />\n<style>\n</style>\n</head>\n<body>\n";
            var buffer = Encoding.GetEncoding("windows-1254").GetBytes(header + sb);
            return File(buffer, "application/vnd.ms-excel");
        }

        [HttpGet]
        public ViewResult Add()
        {
            ViewBag.Tags = _tagRepo.AsQueryable().ToList();
            
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Add(ProductModel model)
        {
            ViewBag.Tags = _tagRepo.AsQueryable().ToList();

            var time = DateTime.Now;

            if (string.IsNullOrEmpty(model.Price.ToString(CultureInfo.InvariantCulture))
                && string.IsNullOrEmpty(model.Title)
                && string.IsNullOrEmpty(model.Name)
                && string.IsNullOrEmpty(model.Currency)
                && string.IsNullOrEmpty(model.Description)
                && (model.Tags.Count <= 0)
                && string.IsNullOrEmpty(model.Code))
            {
                model.Msg = "Girilmemiş alanlar mevcut";
                return View(model);
            }

            if (_productRepo.AsQueryable().Any(w => w.Name == model.Name && w.Title == model.Title && !w.IsDeleted))
            {
                model.Msg = "Kayıt daha önceden oluşturulmuş";
                return View(model);
            }

            var savePath = string.Empty;

            var imgUrls = new List<string>();
            var imgUrlBigs = new List<string>();
            var imgUrlSmalls = new List<string>();

            var imgurlsmall = string.Empty;
            var imgurl960 = string.Empty;

            if (model.Images != null)
            {
                var imgCount = model.Images.Length;
                for (int i = 0; i < imgCount; i++)
                {
                    if (model.Images[i].ContentLength > 0)
                    {
                        try
                        {
                            using (var img = Image.FromStream(model.Images[i].InputStream))
                            {
                                if (img.Width < 960)
                                {
                                    model.Msg += model.Images[i].FileName + " resmin genişliği 960 px den küçük olamaz.Güncellemek için <a href='/admin/product/edit/" + model.Id + "'>tıklayınız</a>";
                                    continue;
                                }

                                if (img.RawFormat.Equals(ImageFormat.Png) ||
                                    img.RawFormat.Equals(ImageFormat.Jpeg))
                                {
                                    var fileName = string.Format("{0}-{1}-{2}.jpg", model.Code, model.Title.ToUrlSlug(), i + 1);
                                    savePath = "/s/img/bodrum/" + fileName;
                                    imgUrlBigs.Add(savePath);
                                    var path = Path.Combine(Server.MapPath("/s/img/product/"), fileName);
                                    model.Images[i].SaveAs(path);

                                    //ImageWatermark(path);

                                    imgurlsmall = string.Format("/s/img/product/k{0}", fileName);
                                    imgurl960 = string.Format("/s/img/product/b{0}", fileName);

                                    FileHelper.SaveImageAs250(
                                        model.Images[i],
                                        Path.Combine(Server.MapPath("/s/img/product/"), string.Format("k{0}", fileName)));

                                    var path960 = Path.Combine(Server.MapPath("/s/img/product/"), string.Format("b{0}", fileName));

                                    FileHelper.SaveImageAs960(
                                        model.Images[i],
                                        path960);

                                    //ImageWatermark(path960);

                                    imgUrlSmalls.Add(imgurlsmall);
                                    imgUrls.Add(imgurl960);
                                }
                            }
                        }
                        catch { }
                    }
                }
            }

            var product = new Product
            {
                CreatedBy = User.Identity.Name,
                CreatedOn = time,
                Description = model.Description,
                Code = model.Code,
                HtmlDescription = model.HtmlDescription,
                IsDeleted = false,
                IsSold = false,
                Title = model.Title,
                Tags = model.Tags,
                UpdatedOn = time,
                UpdatedBy = User.Identity.Name,
                Price = model.Price,
                Currency = model.Currency,

                ImageUrls = imgUrls,
                ImageUrlBigs = imgUrlBigs,
                ImageUrlSmalls = imgUrlSmalls,

                ImageUrlPrimary = imgUrls.Any() ? imgUrls.OrderBy(x => x).First() : string.Empty,
                ImageUrlPrimarySmall = imgUrlSmalls.Any() ? imgUrlSmalls.OrderBy(x => x).First() : string.Empty,
                ImageUrlPrimaryBig = imgUrlBigs.Any() ? imgUrlBigs.OrderBy(x => x).First() : string.Empty,
                VideoUrl = model.VideoUrl
            };

            
            var result = _productRepo.Add(product);

            if (result.Ok)
            {
                //CreateSiteMap();
                return Redirect("/admin");
            }

            model.Msg = "Bir sorun oluştu lütfen tekrar deneyiniz.";
            return View(model);
        }

        #endregion

        #region Tag
        public string GetTagContainer(BsonObjectId productId)
        {
            var result = string.Empty;
            var product = _productRepo.AsQueryable().FirstOrDefault(w => w.Id == productId);
            if (product != null)
            {
                foreach (var tag in product.Tags)
                {
                    result += tag;
                }
            }
            return result;
        }

        [HttpGet]
        public ViewResult Tag()
        {
            var model = new TagListModel { Tags = _tagRepo.AsQueryable().ToList() };
            return View(model);
        }

        [HttpGet]
        public ViewResult TagAdd()
        {
            return this.View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TagAdd(string tagName)
        {
            var model = new TagListModel { Tags = this._tagRepo.AsQueryable().ToList() };

            if (string.IsNullOrEmpty(tagName))
            {
                model.Msg = "Girilmemiş alanlar mevcut";
                return View(model);
            }

            if (model.Tags.Any(x => x.Name == tagName.Trim()))
            {
                model.Msg = "Bölge önceden kayıt edilmiş";
                return View(model);
            }

            var tag = new Tag
            {
                CreatedBy = Consts.System,
                UpdatedBy = Consts.System,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now,
                IsDeleted = false,
                Name = tagName.Trim(),
                UrlName = tagName.ToUrlSlug()
            };
            _tagRepo.Add(tag);
            model.Tags.Add(tag);

            model.Msg = "Etiket Eklendi";

            return RedirectToAction("Tag");
        }

        [HttpPost]
        public JsonResult TagEdit(string id, string name)
        {
            var result = new BaseModel();

            ObjectId tId;
            if (!string.IsNullOrEmpty(id)
                && !string.IsNullOrEmpty(name)
                && ObjectId.TryParse(id, out tId))
            {

                var tag = _tagRepo.AsQueryable().FirstOrDefault(x => x.Id == tId);

                if (!_tagRepo.AsQueryable().Any(x => x.Name == name.Trim())
                    && tag != null)
                {
                    result.OK = _tagRepo.Update(Query<Tag>.EQ(x => x.Id, tId), Update<Tag>.Set(x => x.Name, name)).Ok;

                    if (result.OK)
                    {
                        var products = _productRepo.AsQueryable().Where(w => w.Tags.Contains(tag.Name)).ToList();
                        foreach (var product in products)
                        {
                            var tags = product.Tags;
                            tags.Remove(tag.Name);
                            tags.Add(name);

                            _productRepo.Update(Query<Product>.EQ(w => w.IdStr, product.IdStr),
                                          Update<Product>.Set(w => w.Tags, tags));
                        }
                    }

                }
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult TagDelete(string id)
        {
            var result = new BaseModel();

            ObjectId zId;
            if (!string.IsNullOrEmpty(id)
                && ObjectId.TryParse(id, out zId))
            {
                var tag = _tagRepo.AsQueryable().FirstOrDefault(x => x.Id == zId);

                if (tag != null)
                {
                    if (!_productRepo.AsQueryable().Any(w => w.Tags.Contains(tag.Name) && !w.IsDeleted && !w.IsSold))
                    {
                        result.OK = _tagRepo.Delete(tag).Ok;
                    }
                    else
                    {
                        result.OK = false;
                        result.Msg = "Etikete girilmiş aktif ürünler mevcut lütfen öncelikle onları temizleyiniz";
                    }
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Login
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
        #endregion
    }
}
