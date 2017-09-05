using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AverageRandom : MonoBehaviour {

    InputField averageInput, numInput, minInput, maxInput;
    Button startButton, clearButton;
    Text outputText;

	// Use this for initialization
	void Start () {

        averageInput = this.transform.FindChild("Average/InputField").GetComponent<InputField>();
        numInput = this.transform.FindChild("Num/InputField").GetComponent<InputField>();
        minInput = this.transform.FindChild("Min/InputField").GetComponent<InputField>();
        maxInput = this.transform.FindChild("Max/InputField").GetComponent<InputField>();
        startButton = this.transform.FindChild("StartButton").GetComponent<Button>();
        clearButton = this.transform.FindChild("ClearButton").GetComponent<Button>();
        outputText = this.transform.FindChild("Output").GetComponent<Text>();

        startButton.onClick.AddListener(Calulate);
        clearButton.onClick.AddListener(Clear);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void Clear()
    {
        outputText.text = "";
    }

    void Calulate()
    {
        outputText.text = "";
        if (averageInput.text == "")
        {
            outputText.text = "请输入平均值";
            return;
        }

        if (minInput.text == "")
        {
            outputText.text = "请输入最小值";
            return;
        }

        if (maxInput.text == "")
        {
            outputText.text = "请输入最大值";
            return;
        }

        if (numInput.text == "")
        {
            outputText.text = "请输入数量";
            return;
        }

        float average = float.Parse(averageInput.text);
        float min = float.Parse(minInput.text);
        float max = float.Parse(maxInput.text);
        int num = int.Parse(numInput.text);

        float targetSum = average * num, currentSum = 0;

        List<float> randomList = new List<float>();

        // 随机值
        for (int i = 0; i < num; ++i)
        {
            float temp = FloatPointOne(Random.Range(min, max));
            randomList.Add(temp);
        }

        // 升序排列
        randomList.Sort();

        // 循环校正值
        Check(ref randomList, targetSum, min, max);

        // 再次确认小数点留一位
        for (int i = 0; i < randomList.Count; ++i)
        {
            randomList[i] = FloatPointOne(randomList[i]);
            currentSum += randomList[i];
        }

        currentSum = FloatPointOne(currentSum);

        // 升序排列
        randomList.Sort();

        // 再次校验
        if (Mathf.Abs(targetSum - currentSum) >= 0.1f)
        {
            if (currentSum < targetSum)
            {
                randomList[0] += targetSum - currentSum;
            }
            else if (currentSum > targetSum)
            {
                randomList[randomList.Count - 1] -= currentSum - targetSum;
            }

            // 升序排列
            randomList.Sort();
        }

        currentSum = 0;

        for (int i = 0; i < randomList.Count; ++i)
        {
            currentSum += randomList[i];
            outputText.text += randomList[i] + " || ";
        }

        currentSum = FloatPointOne(currentSum);

        if (Mathf.Abs(targetSum - currentSum) < 0.1f)
        {
            outputText.text += "\n生成成功！";
        }
        else {
            outputText.text += "\n生成错误！";
        }
    }

    void Check(ref List<float> list, float targetNum, float min, float max)
    {
        float currentSum = 0;
        for (int i = 0; i < list.Count; ++i)
        {
            currentSum += list[i];
        }

        if (currentSum > targetNum)
        {
            float diff = currentSum - targetNum;
            if ((list[list.Count - 1] - diff) >= min)
            {
                list[list.Count - 1] -= diff;
            }
            else
            {
                float temp = FloatPointOne(Random.Range(min, list[list.Count - 1]));
                list[list.Count - 1] = temp;
                list.Sort();
                Check(ref list, targetNum, min, max);
            }
        }
        else if (currentSum < targetNum)
        {
            float diff = targetNum - currentSum;
            if ((list[0] + diff) <= max)
            {
                list[0] += diff;
            }
            else
            {
                float temp = FloatPointOne(Random.Range(list[0], max));
                list[0] = temp;
                list.Sort();
                Check(ref list, targetNum, min, max);
            }
        }
    }

    /// <summary>
    /// float 保留以为小数 
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    float FloatPointOne(float num)
    {
        string temp = num.ToString("0.0");
        return float.Parse(temp);
        //int iNum = (int)(num * 10);
        //float fRet = (float)iNum / 10;
        //return fRet;
    }
}
