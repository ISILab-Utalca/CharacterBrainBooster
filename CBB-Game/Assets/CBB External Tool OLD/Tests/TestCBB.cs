using System;
using System.Collections;
using System.Collections.Generic;
using CBB.Lib;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class Test_Evaluator
{
    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Normalize_Evaluator()
    {
        var eva = new Normalize(UnityEngine.Random.Range(0, 50), 0, UnityEngine.Random.Range(50, 100));
        var pth = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<Normalize>(pth, "Eva", "json", eva);
        var rEva = Utility.JSONDataManager.LoadData<Normalize>(pth, "Eva", "json");
        Assert.That(rEva.ToString() == eva.ToString());
    }

    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Multiply_Evaluator()
    {
        var mult = new Multiply(UnityEngine.Random.Range(1, 100), UnityEngine.Random.Range(1, 100));
        var pth = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<Multiply>(pth, "Mult", "json", mult);
        var rMult = Utility.JSONDataManager.LoadData<Multiply>(pth, "Mult", "json");
        Assert.That(rMult.ToString() == mult.ToString());
    }

    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Divide_Evaluator()
    {
        var div = new Divide(UnityEngine.Random.Range(1, 100), UnityEngine.Random.Range(1, 100));
        var pth = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<Divide>(pth, "Div", "json", div);
        var rDiv = Utility.JSONDataManager.LoadData<Divide>(pth, "Div", "json");
        Assert.That(rDiv.ToString() == div.ToString());
    }

    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Identity_Evaluator()
    {
        var ident = new Identity(UnityEngine.Random.Range(0, 100));
        var pth = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<Identity>(pth, "Ident", "json", ident);
        var rIdent = Utility.JSONDataManager.LoadData<Identity>(pth, "Ident", "json");
        Assert.That(rIdent.ToString() == ident.ToString());
    }

    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Distance_V1_Evaluator()
    {
        var dv1 = new DistanceV1(UnityEngine.Random.Range(0, 100), UnityEngine.Random.Range(0, 100));
        var pth = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<DistanceV1>(pth, "Dv1", "json", dv1);
        var rDv1 = Utility.JSONDataManager.LoadData<DistanceV1>(pth, "Dv1", "json");
        Assert.That(rDv1.ToString() == dv1.ToString());
    }

    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Distance_V2_Evaluator()
    {
        var dv2 = new DistanceV2(
            new Vector2(UnityEngine.Random.Range(0, 100), UnityEngine.Random.Range(0, 100)),
            new Vector2(UnityEngine.Random.Range(0, 100), UnityEngine.Random.Range(0, 100))
            );
        var pth = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<DistanceV2>(pth, "Dv2", "json", dv2);
        var rDv2 = Utility.JSONDataManager.LoadData<DistanceV2>(pth, "Dv2", "json");
        Assert.That(rDv2.ToString() == dv2.ToString());
    }

    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Distance_V3_Evaluator()
    {
        var dv3 = new DistanceV3(
            new Vector3(UnityEngine.Random.Range(0, 100), UnityEngine.Random.Range(0, 100), UnityEngine.Random.Range(0, 100)),
            new Vector3(UnityEngine.Random.Range(0, 100), UnityEngine.Random.Range(0, 100), UnityEngine.Random.Range(0, 100))
            );
        var pth = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<DistanceV3>(pth, "Dv3", "json", dv3);
        var rDv3 = Utility.JSONDataManager.LoadData<DistanceV3>(pth, "Dv3", "json");
        Assert.That(rDv3.ToString() == dv3.ToString());
    }
}

public class Test_Variables
{
    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Variable_Float()
    {
        var v = new Variable("Var_Float", typeof(float), typeof(MonoBehaviour));
        var pth = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<Variable>(pth, "Var_Float", v);
        var rV = Utility.JSONDataManager.LoadData<Variable>(pth, "Var_Float");
        Assert.That(rV.ToString() == v.ToString());
    }

    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Variable_String()
    {
        var v = new Variable("Var_String", typeof(string), typeof(MonoBehaviour));
        var pth = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<Variable>(pth, "Var_String", v);
        var rV = Utility.JSONDataManager.LoadData<Variable>(pth, "Var_String");
        Assert.That(rV.ToString() == v.ToString());
    }

    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Variable_Bool()
    {
        var v = new Variable("Var_Bool", typeof(bool), typeof(MonoBehaviour));
        var pth = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<Variable>(pth, "Var_Bool", v);
        var rV = Utility.JSONDataManager.LoadData<Variable>(pth, "Var_Bool");
        Assert.That(rV.ToString() == v.ToString());
    }

    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Variable_Vector2()
    {
        var v = new Variable("Var_Vec2", typeof(Vector2), typeof(MonoBehaviour));
        var pth = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<Variable>(pth, "Var_Vec2", v);
        var rV = Utility.JSONDataManager.LoadData<Variable>(pth, "Var_Vec2");
        Assert.That(rV.ToString() == v.ToString());
    }

    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Variable_Vector3()
    {
        var v = new Variable("Var_Vec3", typeof(Vector3), typeof(MonoBehaviour));
        var pth = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<Variable>(pth, "Var_Vec3", v);
        var rV = Utility.JSONDataManager.LoadData<Variable>(pth, "Var_Vec3");
        Assert.That(rV.ToString() == v.ToString());
    }
}

public class Test_Curves
{
    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Linear_Curve()
    {
        var curve = new Linear(0.5f);
        var path = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<Linear>(path, "Linear", curve);
        var rC = Utility.JSONDataManager.LoadData<Linear>(path, "Linear");
        Assert.That(rC.ToString() == curve.ToString());
    }

    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Exponential_Curve()
    {
        var curve = new Exponencial(0.5f);
        var path = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<Exponencial>(path, "Exponencial", curve);
        var rC = Utility.JSONDataManager.LoadData<Exponencial>(path, "Exponencial");
        Assert.That(rC.ToString() == curve.ToString());
    }

    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Inverse_Exponential_Curve()
    {
        var curve = new ExponencialInvertida(0.5f);
        var path = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<ExponencialInvertida>(path, "ExponencialInvertida", curve);
        var rC = Utility.JSONDataManager.LoadData<ExponencialInvertida>(path, "ExponencialInvertida");
        Assert.That(rC.ToString() == curve.ToString());
    }

    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Staggered_Curve()
    {
        var curve = new Staggered(0.5f);
        var path = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<Staggered>(path, "Staggered", curve);
        var rC = Utility.JSONDataManager.LoadData<Staggered>(path, "Staggered");
        Assert.That(rC.ToString() == curve.ToString());
    }

    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Sigmoid_Curve()
    {
        var curve = new Sigmoide(0.5f);
        var path = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<Sigmoide>(path, "Sigmoide", curve);
        var rC = Utility.JSONDataManager.LoadData<Sigmoide>(path, "Sigmoide");
        Assert.That(rC.ToString() == curve.ToString());
    }

    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Constant_Curve()
    {
        var curve = new Constant(0.5f);
        var path = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<Constant>(path, "Constant", curve);
        var rC = Utility.JSONDataManager.LoadData<Constant>(path, "Constant");
        Assert.That(rC.ToString() == curve.ToString());
    }

    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Bell_Curve()
    {
        var curve = new Bell(0.5f);
        var path = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<Bell>(path, "Bell", curve);
        var rC = Utility.JSONDataManager.LoadData<Bell>(path, "Bell");
        Assert.That(rC.ToString() == curve.ToString());
    }
}

public class TestCBB
{
    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_Consideration()
    {
        var vf = new Variable("Var_1", typeof(float), typeof(MonoBehaviour));
        var vv3 = new Variable("Var_2", typeof(Vector3), typeof(MonoBehaviour));
        var vs = new Variable("Var_3", typeof(string), typeof(MonoBehaviour));
        var vb = new Variable("Var_4", typeof(bool), typeof(MonoBehaviour));
        var eva = new Normalize(50, 0, 100);
        var curve = new Linear(0.5f);
        var cons = new Consideration("Cons_1", true, new List<Variable>() { vf, vv3, vs, vb }, eva, curve);
        var pth1 = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<Consideration>(pth1, "Cons", "json", cons);
        var rC = Utility.JSONDataManager.LoadData<Consideration>(pth1, "Cons", "json");
        Assert.That(cons.ToString() == rC.ToString());
    }

    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_AgentData()
    {
        var vf = new Variable("Var_1", typeof(float), typeof(MonoBehaviour));
        var vv3 = new Variable("Var_2", typeof(Vector3), typeof(MonoBehaviour));
        var inputs = new List<Variable>() { vf, vv3 };
        var a1 = new ActionInfo("Action_1", typeof(MonoBehaviour));
        var a2 = new ActionInfo("Action_1", typeof(MonoBehaviour));
        var action = new List<ActionInfo>() { a1, a2 };
        var data = new AgentData(typeof(MonoBehaviour), inputs, action);
        var path = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<AgentData>(path, "AgentData", "json", data);
        var rd = Utility.JSONDataManager.LoadData<AgentData>(path, "AgentData", "json");
        Assert.That(data.ToString() == rd.ToString());
    }

    [Test(Author = "Nicolas Romero")]
    public void Can_Save_And_Load_AgentBrainData()
    {
        // Variables
        var vf = new Variable("Var_1", typeof(float), typeof(MonoBehaviour));
        var vv3 = new Variable("Var_2", typeof(Vector3), typeof(MonoBehaviour));
        var inputs = new List<Variable>() { vf, vv3 };

        // Actions
        var a1 = new ActionInfo("Action_1", typeof(MonoBehaviour));
        var a2 = new ActionInfo("Action_1", typeof(MonoBehaviour));
        var actions = new List<ActionInfo>() { a1, a2 };

        // AgentData
        var data = new AgentData(typeof(MonoBehaviour), inputs, actions);

        // Evaluation
        var eva = new Normalize(50, 0, 100);

        // Curve
        var curve = new Linear(0.5f);

        // Considerations
        var cons1 = new Consideration("Cons_1", true, new List<Variable>() { vf, vv3 }, eva, curve);
        var cons2 = new Consideration("Cons_1", true, new List<Variable>() { vf }, eva, curve);
        var considerations = new List<Consideration> { cons1, cons2 };

        // ActionUtilities
        var au1 = new ActionUtility("ActionUtility_1", a1, eva, curve, new List<Variable>() { vf, vv3 }, considerations);
        var au2 = new ActionUtility("ActionUtility_2", a1, eva, curve, new List<Variable>() { vf }, considerations);

        // BrainData
        var brain = new AgentBrainData(data, considerations, new List<ActionUtility>() { au1, au2 });
        var path = Application.dataPath + "/Git-Ignore/Test";
        Utility.JSONDataManager.SaveData<AgentBrainData>(path, "AgentBrainData", "json", brain);
        var rB = Utility.JSONDataManager.LoadData<AgentBrainData>(path, "AgentBrainData", "json");
        Assert.That(brain.ToString() == rB.ToString());
    }
}

