using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringColor {
    static string aqua      = "<color=aqua>";
    static string blue      = "<color=blue>";
    static string brown     = "<color=brown>";
    static string cyan      = "<color=cyan>";
    static string darkblue  = "<color=darkblue>";
    static string magenta   = "<color=magenta>";
    static string green     = "<color=green>";
    static string grey      = "<color=grey>";
    static string lightblue = "<color=lightblue>";
    static string lime      = "<color=lime>";
    static string maroon    = "<color=maroon>";
    static string navy      = "<color=navy>";
    static string olive     = "<color=olive>";
    static string orange    = "<color=orange>";
    static string purple    = "<color=purple>";
    static string red       = "<color=red>";
    static string silver    = "<color=silver>";
    static string teal      = "<color=teal>";
    static string white     = "<color=white>";
    static string yellow    = "<color=yellow>";
    static string black     = "<color=black>";
    static string endColor  = "</color>"; 

    public static string Aqua       { get { return aqua; } }
    public static string Blue       { get { return blue; } }
    public static string Brown      { get { return brown; } }
    public static string Cyan       { get { return cyan; } }
    public static string Darkblue   { get { return darkblue; } }
    public static string Magenta    { get { return magenta; } }
    public static string Green      { get { return green; } }
    public static string Grey       { get { return grey; } }
    public static string Lightblue  { get { return lightblue; } }
    public static string Lime       { get { return lime; } }
    public static string Maroon     { get { return maroon; } }
    public static string Navy       { get { return navy; } }
    public static string Olive      { get { return olive; } }
    public static string Orange     { get { return orange; } }
    public static string Purple     { get { return purple; } }
    public static string Red        { get { return red; } }
    public static string Silver     { get { return silver; } }
    public static string Teal       { get { return teal; } }
    public static string White      { get { return white; } }
    public static string Yellow     { get { return yellow; } }
    public static string Black      { get { return black; } }
    public static string EndColor   { get { return endColor; } }

    public static string Random()
	{
		int randNum = UnityEngine.Random.Range(0, 21);
		return GetColorFormIndex(randNum);
	}

	public static string GetColorFormIndex(int randNum)
	{
		switch (randNum)
		{
			case 0: return Aqua;
			case 1: return Blue;
			case 2: return Brown;
			case 3: return Cyan;
			case 4: return Darkblue;
			case 5: return Magenta;
			case 6: return Green;
			case 7: return Grey;
			case 8: return Lightblue;
			case 9: return Lime;
			case 10: return Maroon;
			case 11: return Navy;
			case 12: return Olive;
			case 13: return Orange;
			case 14: return Purple;
			case 15: return Red;
			case 16: return Silver;
			case 17: return Teal;
			case 18: return White;
			case 19: return Yellow;
            case 20: return Black;
			default:
				return Aqua;
		}
	}
}
