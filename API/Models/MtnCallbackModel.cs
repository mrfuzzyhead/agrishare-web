using Agrishare.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Agrishare.API.Models
{
    public class MtnCallbackModel
    {
        public int externalId { get; set; }
        public string status { get; set; }
    }
}