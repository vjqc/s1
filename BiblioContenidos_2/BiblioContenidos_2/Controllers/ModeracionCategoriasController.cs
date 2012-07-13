using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using BiblioContenidos_2.Models;
using Trirand.Web.Mvc;

namespace BiblioContenidos_2.Controllers
{
    public class ModeracionCategoriasController : Controller
    {
        //
        // GET: /ModeracionCategorias/

        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult Index()
        {
            var gridModel = new ModeracionCategorias();
            var catesGrid = gridModel.CategoriasGrid;

            SetUpGridCategorias(catesGrid);

            return View(gridModel);
        }

        private void SetUpGridCategorias(JQGrid catesGrid)
        {
            catesGrid.ID = "CategoriasGrid";
            catesGrid.DataUrl = Url.Action("DatosCategorias");
            catesGrid.EditUrl = Url.Action("EditarFilas");

            catesGrid.ToolBarSettings.ShowSearchToolBar = true;
            catesGrid.Columns.Find(c => c.DataField == "Id").Searchable = false;
            catesGrid.Columns.Find(c => c.DataField == "Estado").Searchable = false;

            SetDescripcionSearchDropDown(catesGrid); //no es up, es text

            catesGrid.ToolBarSettings.ShowEditButton = true;
            catesGrid.ToolBarSettings.ShowAddButton = true;
            catesGrid.ToolBarSettings.ShowDeleteButton = true;
            catesGrid.EditDialogSettings.CloseAfterEditing = true;
            catesGrid.AddDialogSettings.CloseAfterAdding = true;

            SetUpEstadoEditDropDown(catesGrid);
        }

        private void SetDescripcionSearchDropDown(JQGrid catesGrid)
        {
            JQGridColumn DescripcionColumn = catesGrid.Columns.Find(c => c.DataField == "Descripcion");
            DescripcionColumn.Searchable = true;

            DescripcionColumn.DataType = typeof(string);
            DescripcionColumn.SearchToolBarOperation = SearchOperation.Contains;
            DescripcionColumn.SearchType = SearchType.TextBox;

            if (catesGrid.AjaxCallBackMode == AjaxCallBackMode.RequestData)
            {
                var BiblioModel = new DataClasses1DataContext();

            }
        }

        public JsonResult DatosCategorias()
        {
            var gridModel = new ModeracionCategorias();
            var db = new DataClasses1DataContext();

            SetUpGridCategorias(gridModel.CategoriasGrid);

            return gridModel.CategoriasGrid.DataBind(db.Categorias);

        }

        public ActionResult EditarFilas(Categoria catego)
        {
            var gridModel = new ModeracionCategorias();
            var db = new DataClasses1DataContext();

            if (gridModel.CategoriasGrid.AjaxCallBackMode == AjaxCallBackMode.EditRow)
            {
                //int yy = 8;
                Categoria NuevaCatego = (from o in db.Categorias
                                         where o.Id == catego.Id
                                         select o).First<Categoria>();

                NuevaCatego.Descripcion = catego.Descripcion;
                NuevaCatego.Estado = catego.Estado;
                db.SubmitChanges();
            }

            if (gridModel.CategoriasGrid.AjaxCallBackMode == AjaxCallBackMode.AddRow)
            {
                Categoria NuevaCatego = new Categoria();

                NuevaCatego.Descripcion = catego.Descripcion;
                NuevaCatego.Estado = catego.Estado;

                db.Categorias.InsertOnSubmit(NuevaCatego);
                db.SubmitChanges();
            }

            if (gridModel.CategoriasGrid.AjaxCallBackMode == AjaxCallBackMode.DeleteRow)
            {
                Categoria NuevaCat = db.Categorias.Single(o => o.Id == catego.Id);

                //Javier, Eliminar antes las relaciones Contenidos-Categorias
                List<RelContenidosCategoria> lista_rel = db.RelContenidosCategorias.Where(c => c.IdCategoria == catego.Id).ToList();
                db.RelContenidosCategorias.DeleteAllOnSubmit(lista_rel);
                db.SubmitChanges();

                db.Categorias.DeleteOnSubmit(NuevaCat);
                db.SubmitChanges();
            }

            return RedirectToAction("Index", "ModeracionCategorias");
        }

        public void SetUpEstadoEditDropDown(JQGrid catesGrid)
        {
            JQGridColumn estadosColumn = catesGrid.Columns.Find(c => c.DataField == "Estado");
            estadosColumn.Editable = true;
            estadosColumn.EditType = EditType.DropDown;

            if (catesGrid.AjaxCallBackMode == AjaxCallBackMode.RequestData)
            {
                List<string> estados = new List<string>() { "Aceptado", "Rechazado", "Pendiente" };

                var editList = from est in estados
                               select new SelectListItem
                               {
                                   Text = est,
                                   Value = est
                               };

                estadosColumn.EditList = editList.ToList<SelectListItem>();

            }
        }

    }
}
