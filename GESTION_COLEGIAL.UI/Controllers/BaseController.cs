﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class BaseController:Controller
    {
        protected string msjExist = $"El registro ya está en uso.";



        protected void Show(AlertMessageType type, string message=null)
        {
            ViewBag.JavaScriptFunction = string.Format($"alertConfig.alert('{message}', '{type}');");
        }

        protected void ShowController(AlertMessageType type)
        {
            switch (type)
            {
                case AlertMessageType.Success:
                    Show(AlertMessageType.Success, "Registro guardado exitosamente.");//satifactoriamente
                    break;
                case AlertMessageType.Info:
                    Show(AlertMessageType.Info, "");
                    break;
                case AlertMessageType.Warning:
                    Show(AlertMessageType.Warning, "Ha ocurrido un problema.");
                    break;
                case AlertMessageType.Error:
                    Show(AlertMessageType.Error, "Se produjo un error inesperado.");
                    break;
            }
        }


        public enum AlertMessageType
        {
            Info = 0,
            Warning = 1,
            Success = 2,
            Error = 3

        }
    }
}