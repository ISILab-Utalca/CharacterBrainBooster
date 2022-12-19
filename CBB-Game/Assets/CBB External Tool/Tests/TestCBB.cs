using System.Collections;
using System.Collections.Generic;
using CBB.Lib;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EvaluatorsCBB
{
    private Normalize Create_Normalize_Evaluator()
    {
        var eva = new Normalize(50, 0, 100);
        return eva;
    }

    private Multiply Create_Multiply_Evaluator()
    {
        var eva = new Multiply(5,10);
        return eva;
    }

    private Divide Create_Divide_Evaluator()
    {
        var eva = new Divide(5,10);
        return eva;
    }

    private Identity Create_Identity_Evaluator()
    {
        var eva = new Identity(5f);
        return eva;
    }

    private DistanceV1 Create_DistnaceV1_Evalautor()
    {
        var eva = new DistanceV1(0f, 10f);
        return eva;
    }

    private DistanceV2 Create_DistnaceV2_Evalautor()
    {
        var eva = new DistanceV2(new Vector2(0, 0), new Vector2(10, 10));
        return eva;
    }

    private DistanceV3 Create_DistnaceV3_Evalautor()
    {
        var eva = new DistanceV3(new Vector3(0, 0), new Vector3(10, 10));
        return eva;
    }


    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Normalize_Evaluator()
    {
        var eva = Create_Normalize_Evaluator();
        var pth = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<Normalize>(pth, "Eva", eva);
        var rEva = Utility.JSONDataManager.LoadData<Normalize>(pth, "Eva");
        Assert.That(rEva.ToString() == eva.ToString());
       
    }

    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Multiply_Evaluator()
    {
        var mult = Create_Multiply_Evaluator();
        var pth = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<Multiply>(pth, "Mult", mult);
        var rMult = Utility.JSONDataManager.LoadData<Multiply>(pth, "Mult");
        Assert.That(rMult.ToString() == mult.ToString());
    }

    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Divide_Evaluator()
    {
        var div = Create_Divide_Evaluator();
        var pth = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<Divide>(pth, "Div", div);
        var rDiv = Utility.JSONDataManager.LoadData<Divide>(pth, "Div");
        Assert.That(rDiv.ToString() == div.ToString());
    }

    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Identity_Evaluator()
    {
        var ident = Create_Identity_Evaluator();
        var pth = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<Identity>(pth, "Ident", ident);
        var rIdent = Utility.JSONDataManager.LoadData<Identity>(pth, "Ident");
        Assert.That(rIdent.ToString() == ident.ToString());
    }

    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Distance_V1_Evaluator()
    {
        var dv1 = Create_DistnaceV1_Evalautor();
        var pth = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<DistanceV1>(pth, "Dv1", dv1);
        var rDv1 = Utility.JSONDataManager.LoadData<DistanceV1>(pth, "Dv1");
        Assert.That(rDv1.ToString() == dv1.ToString());
    }

    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Distance_V2_Evaluator()
    {
        var dv2 = Create_DistnaceV2_Evalautor();
        var pth = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<DistanceV2>(pth, "Dv2", dv2);
        var rDv2 = Utility.JSONDataManager.LoadData<DistanceV2>(pth, "Dv2");
        Assert.That(rDv2.ToString() == dv2.ToString());
    }

    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Distance_V3_Evaluator()
    {
        var dv3 = Create_DistnaceV3_Evalautor();
        var pth = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<DistanceV3>(pth, "Dv3", dv3);
        var rDv3 = Utility.JSONDataManager.LoadData<DistanceV3>(pth, "Dv3");
        Assert.That(rDv3.ToString() == dv3.ToString());
    }
}

public class TestCBB
{
    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Variable()
    {
        var v = new Variable("Var_1", typeof(float), typeof(MonoBehaviour), 5f);
        var pth1 = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<Variable>(pth1, "Var1", v);
        var rV = Utility.JSONDataManager.LoadData<Variable>(pth1, "Var1");
        Assert.That(rV.ToString() == v.ToString());
    }

    ///*
    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Consideration()
    {
        var vf = new Variable("Var_1", typeof(float), typeof(MonoBehaviour), 5f);
        var vv3 = new Variable("Var_2", typeof(Vector3), typeof(MonoBehaviour), new Vector3(1,2,3));
        var vs = new Variable("Var_3", typeof(string), typeof(MonoBehaviour), "hola");
        var vb = new Variable("Var_4", typeof(bool), typeof(MonoBehaviour), false);
        var eva = new Normalize(50,0,100);
        var curve = new Linear();
        var cons = new Consideration("Cons_1", new List<Variable>() { vf, vv3, vs, vb }, eva, curve);
    }
    //*/
}

