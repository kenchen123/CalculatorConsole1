using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleApp1
{
    class Program
    {
        static string DefaultFilePath()
        {
            //string filePath = @"C:\Users\<username>\Desktop\sum_of_history.txt"; 
            //C:\Users\<username>\Desktop\sum_of_history.txt 

            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\sum_of_history.txt";
            //string filePath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\sum_of_history.json";
            return filePath;
        }

        static void Help()
        {
            string commands = "All valid keywords:\n" +
                "'q': Quit program.\n" +
                "'sum': Summarize the entered numbers.\n" +
                "'hist': List the calculation history.\n" +
                "'del No.': Delete assigned data. (eg. 'del 2' to delete the No.2 data)\n" +
                "'search': Search from history. (Result and calculation time)";

            Console.WriteLine(commands);
        }

        static string fileFormat(int dictKey, string dictValue)
        {
            var format = "No." + dictKey.ToString() + "|" + dictValue;

            return format;
        }

        static string dictValueFormat(string numbers, double result, DateTime sumTime)
        {
            var format = "Numbers:" + numbers + "\tSum of numbers:" + result.ToString() + "\tTime:" + sumTime;
            return format;
        }

        //Save history to txt file 
        static void SaveToTxt(Dictionary<int, string> history, string txtFilePath)
        {
            var historyArray = new string[history.Count];
            var count = 0;

            foreach (var row in history)
            {
                historyArray[count] = fileFormat(row.Key, row.Value);
                count++;
            }

            System.IO.File.WriteAllLines(txtFilePath, historyArray);
            Console.WriteLine("Saved " + count.ToString() + " records to " + txtFilePath);

        }



        //Load history from txt file 
        //載入完傳回dictionary 
        static Dictionary<int, string> LoadFromTxt(string filePath)
        {
            var counter = 0;
            var record = "";
            var history = new Dictionary<int, string>();

            try
            {

                if (System.IO.File.Exists(filePath))
                {

                    //防呆，限制讀入txt檔 
                    if (filePath.Substring(filePath.Length - 3, 3) == "txt")
                    {

                        System.IO.StreamReader file = new System.IO.StreamReader(filePath);

                        var key = 0;
                        var value = "";
                        while ((record = file.ReadLine()) != null)
                        {
                            System.Console.WriteLine("Loading: " + record);
                            counter++;

                            //拆字(分出key:int 和value:string)、丟回dict 
                            string[] aaa = record.Split('|');
                            int.TryParse(aaa[0].Substring(3, aaa[0].Length - 3), out key);
                            value = aaa[1];
                            history.Add(key, value);

                        }
                        file.Close();
                    }
                    else
                    {
                        Console.WriteLine("Invalid file.");

                    }
                }
                else
                    Console.WriteLine("Invalid file.");

                System.Console.WriteLine("There were {0} records loaded.", counter);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return history;





        }





        //尚未支援多個數字搜尋
        static void SearchNumberFromTxt(Dictionary<int, string> history)
        {
            Console.WriteLine("Please enter sum of number:");
            var inputNumber = Console.ReadLine();
            var searchNumber = 0.0;
            var dictNumber = 0.0;
            var validNumber = false;

            while (!validNumber)
            {
                if (!double.TryParse(inputNumber, out searchNumber))
                {
                    Console.WriteLine("Invalid number. Please try again.");
                    inputNumber = Console.ReadLine();
                }
                else
                    validNumber = true;
            }

            var count = 0;
            foreach (KeyValuePair<int, string> row in history)
            {
                //拆出結果數字
                //搜尋數字和結果數字比對，
                var strValue = row.Value.Substring(row.Value.IndexOf("Sum of numbers") + "Sum of numbers:".Length);
                var arrValue = strValue.Split('\t');

                if (double.TryParse(arrValue[0], out dictNumber))
                {
                    if (searchNumber == dictNumber)
                    {
                        Console.WriteLine(fileFormat(row.Key, row.Value));
                        count++;
                    }
                }
            }
            Console.WriteLine("Found {0} records", count.ToString());

        }

        //尚未支援多個日期搜尋
        static void SearchTimeFromTxt(Dictionary<int, string> history)
        {
            Console.WriteLine("Please enter time (Format:yyyyMMdd. eg.20181123):");
            var inputTime = Console.ReadLine();
            var searchTime = DateTime.Now;
            var dictTime = DateTime.Now.Date;
            var validTime = false;
            var enUS = new CultureInfo("en-US");

            while (!validTime)
            {
                if (!DateTime.TryParseExact(inputTime, "yyyyMMdd", enUS, DateTimeStyles.None, out searchTime))
                {
                    Console.WriteLine("Invalid datetime format. Please try again.");
                    inputTime = Console.ReadLine();
                }
                else
                    validTime = true;
            }

            var count = 0;
            foreach (KeyValuePair<int, string> row in history)
            {
                //拆出結果日期
                //搜尋日期和紀錄日期比對，
                var strValue = row.Value.Substring(row.Value.IndexOf("Time:") + "Time:".Length);
                //var arrValue = strValue.Split('\t');

                if (DateTime.TryParse(strValue, out dictTime))
                {
                    if (searchTime.Date == dictTime.Date)
                    {
                        Console.WriteLine(fileFormat(row.Key, row.Value));
                        count++;
                    }
                }
            }
            Console.WriteLine("Found {0} records", count.ToString());
        }


        private static Func<string, List<double>> parse =
          input => input.Split(' ')
              .Where(x => !string.IsNullOrEmpty(x))
              .Select(x => double.Parse(x))
              .ToList();

        static void Main(string[] args)
        {

            var condition = true;
            var numberLists = new List<double> { };
            var sumOfNumbers = 0.0;
            var count = 0;
            var filePath = DefaultFilePath();




            // var calculationHistory = new Dictionary<int, string>();
            // calculationHistory = LoadFromTxt(filePath, calculationHistory);
            //上下效果相同，不須額外在這裡new，在fuction那邊處理即可
            //另， 若要 LoadFromTxt(filePath, calculationHistory)
            //則calculationHistory = LoadFromTxt(filePath, calculationHistory);  
            //改成LoadFromTxt(filePath, calculationHistory);
            //而LoadFromTxt改void、點掉return、不須在改成LoadFromTxt中new dictionary

            var calculationHistory = LoadFromTxt(filePath);
            if (calculationHistory.Count > 0)
                count = calculationHistory.Keys.Last();

            while (condition)
            {
                Console.WriteLine("\nPlease enter a number, it can add up all the entered numbers\n " +
                    "[enter 'q' to quit, 'help' to see other commands]:");
                var inputString = Console.ReadLine();

                if (inputString == "q")
                    break;

                else if (inputString == "help")
                {
                    Help();
                    //continue;
                }

                //加總sum，加總完立即存到txt 
                else if (inputString == "sum")
                {
                    var strNumbers = "";
                    foreach (var number in numberLists)
                    {
                        sumOfNumbers += number;
                        strNumbers += number.ToString() + ", ";
                    }

                    Console.WriteLine("The sum of the entered numbers is {0}", sumOfNumbers);

                    count++;
                    strNumbers = strNumbers.Remove(strNumbers.Length - 2) + ".";

                    calculationHistory.Add(count, dictValueFormat(strNumbers, sumOfNumbers, DateTime.Now));
                    SaveToTxt(calculationHistory, filePath); //存入txt 

                    sumOfNumbers = 0;
                    numberLists.Clear();


                    SaveToTxt(calculationHistory, filePath);



                }

                //歷史查詢hist 
                else if (inputString == "hist")
                {
                    //foreach (KeyValuePair<int, List<string>> row in calculationHistory) 
                    foreach (KeyValuePair<int, string> row in calculationHistory)
                    {
                        //Console.WriteLine (format: "No.{0} - " +"{1}", arg0: row.Key, arg1: row.Value); 
                        Console.WriteLine(fileFormat(row.Key, row.Value));
                    }
                    Console.WriteLine(calculationHistory.Keys.Count.ToString() + " records");
                    // continue;
                }

                //刪除指定資料del 
                else if (inputString.Contains("del"))
                {
                    //以','分隔數字，支援多筆刪除 
                    string[] waitToDeletes = inputString.Substring(4, inputString.Length - 4).Split(',');
                    var validNumber = true;
                    //檢查輸入字串是否有效，非整數者跳出錯誤提示 
                    foreach (var waitToDelete in waitToDeletes)
                    {
                        var i = 0;
                        if (!int.TryParse(waitToDelete, out i) == true)
                        {
                            Console.WriteLine("Include invalid No.. Please try again.");
                            validNumber = false;
                        }
                    }

                    if (validNumber == false)
                        continue;

                    //檢查完成後，跳出是否確認刪除提示 
                    //輸入'Y'開始刪除，'N'或其他不做任何事 
                    Console.WriteLine("Are you sure to delete? ('Y' to delete, 'N' to cancel)");
                    inputString = Console.ReadLine();
                    if (inputString.ToUpper() == "Y")
                    {
                        foreach (var waitToDelete in waitToDeletes)
                        {
                            var i = 0;
                            int.TryParse(waitToDelete, out i);
                            calculationHistory.Remove(i);
                        }

                        SaveToTxt(calculationHistory, filePath);
                        Console.WriteLine("Deletion finished.");
                        //continue;
                    }

                }

                //搜尋功能
                //可用關鍵字:結果、時間
                else if (inputString == "search")
                {

                    var validSearchType = false;

                    Console.WriteLine("Use 'sum of number' or 'time' to search?\n" +
                        "['s' to use sum of number, 't' to use time, 'q' to quit search mode.]");

                    //預計加入模式切換
                    while (!validSearchType)
                    {

                        var inputSearchType = Console.ReadLine();

                        if (inputSearchType.ToUpper() == "Q")
                        {
                            validSearchType = true;
                            continue;
                        }
                        else if (inputSearchType.ToUpper() == "S")
                        {
                            SearchNumberFromTxt(calculationHistory);
                            Console.WriteLine("['s' to use sum of number, 't' to use time, 'q' to quit search mode.]");
                        }
                        else if (inputSearchType.ToUpper() == "T")
                        {
                            SearchTimeFromTxt(calculationHistory);
                            Console.WriteLine("['s' to use sum of number, 't' to use time, 'q' to quit search mode.]");
                        }
                        else
                            Console.WriteLine("Invalid search type. Please try again." +
                                "['s' to use sum of number, 't' to use time, 'q' to quit search mode.]");

                    }

                }
                //輸入非關鍵字，若是數字則加入，其他則報錯 
                else
                {
                    try
                    {
                        var value = Convert.ToDouble(inputString);
                        numberLists.Add(value);
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine("ERROR: {0}", e.ToString()); 
                        Console.WriteLine("Not a number, please enter again.");
                    }
                }
            }
        }
    }


}