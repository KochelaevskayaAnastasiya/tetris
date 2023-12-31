﻿using System.ComponentModel.DataAnnotations;

namespace tetris
{
    public class NewGlass
    {
        [Required(ErrorMessage = "Необходимо заполнить поле \"Высота\"")]
        [Range(20, 40, ErrorMessage = "Значение поля \"Высота\" должно находиться в диапозоне от {1} до {2}")]
        public string Height { get; set; }

        [Required(ErrorMessage = "Необходимо заполнить поле \"Ширина\"")]
        [Range(10, 20, ErrorMessage = "Значение поля \"Ширина\" должно находиться в диапозоне от {1} до {2}")]
        public string Width { get; set; }

        public int err { get; set; }

    }
}
