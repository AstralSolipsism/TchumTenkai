using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Flags]
public enum ManifestationTag
{
    无 = 0,
    能量 = 1 << 0,  
    物质 = 1 << 1,  
    元素 = 1 << 2   
    
}