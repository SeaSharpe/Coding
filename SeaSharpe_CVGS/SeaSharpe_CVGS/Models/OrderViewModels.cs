﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeaSharpe_CVGS.Models
{
    public class OrderHistoryViewModel
    {
        public OrderHistoryViewModel()
        {

        }
        public OrderHistoryViewModel(DateTime opd, DateTime sd, string gn, string pn, decimal pp, int id)
        {
            this.orderPlacementDate = opd;
            this.shipDate = sd;
            this.gameName = gn;
            this.platform = pn;
            this.pricePaid = pp;
            this.id = id;
        }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyy}")]
        [Display(Name = "Order Date")]
        public DateTime orderPlacementDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyy}")]
        [Display(Name = "Ship Date")]
        public DateTime shipDate { get; set; }

        [Display(Name = "Name")]
        public string gameName { get; set; }

        [Display(Name = "Platform")]
        public string platform { get; set; }

        [Display(Name = "Price")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal pricePaid { get; set; }

        public int id  { get; set; }
    }

    public class CartViewModel
    {
        public CartViewModel() 
        { 
        
        }
        public CartViewModel(OrderItem item, bool remove)
        {
            this.item = item;
            this.remove = remove;
        }

        public OrderItem item { get; set; }

        [Display(Name = "Remove")]
        public bool remove { get; set; }
    }
}