using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arycs_Fe.Models
{
    public class Character
    {
        public Sys_CharacterEntity info { get; private set; }

        public Character(Sys_CharacterEntity info)
        {
            this.info = info;
        }
    }
}