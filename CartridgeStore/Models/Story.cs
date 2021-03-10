using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CartridgeStore.Models
{
    public class Story
    {
        public Story() { }

        public Story(DateTime dateTime, bool isSupply, Unit unit, IEnumerable<Cartridge> cartridges)
        {
            DateTime   = dateTime;
            IsSupply   = isSupply;
            Unit       = unit;
            Cartridges = new List<ExcludedCartridge>();
            foreach (Cartridge cartridge in cartridges)
            {
                Cartridges.Add(new ExcludedCartridge(cartridge.BarCode, cartridge.Manufacturer, cartridge.Model, cartridge.Qty));
            }
        }

        [Display(AutoGenerateField = false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid StoryId { get; set; }

        [Display(Name = "Дата и Время")]
        [DataType(DataType.DateTime)]
        public DateTime DateTime { get; set; }

        [Display(Name = "Действие")]
        public bool IsSupply { get; set; }

        [Display(Name = "Подразделение")]

        public Unit Unit { get; set; }

        public List<ExcludedCartridge> Cartridges { get; set; }
    }
}