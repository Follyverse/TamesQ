using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Tames.Scripts.Markers
{
    public class Marktest:MonoBehaviour
    {
        [SerializeField]
        [TextAreaAttribute(5, 10)]
        private string[] names;
    }
}
