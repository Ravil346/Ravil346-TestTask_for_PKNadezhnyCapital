using System;
//using System.Collections.Generic;
//using System.Globalization;
using System.IO;
using System.Net.Http;
//using System.Text;
using System.Threading.Tasks;
//using NBitcoin;
//using NBitcoin.DataEncoders;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Aspose.Cells;
using Aspose.Cells.Utility;

namespace BlockProject
{
    internal class Program
    {

        public class blockOfChain
        {
            public int Height { get; set; }
            public string Hash { get; set; }
            public int Time = 0;
           
        }


        static async Task Main(string[] args)
        {
            string latestBlockUrl = "https://blockchain.info/latestblock";
            string blockDataUrl = "https://blockchain.info/rawblock/";

            using (HttpClient client = new HttpClient())
            {
                // Получаем данные о последнем блоке
                HttpResponseMessage response = await client.GetAsync(latestBlockUrl);
                response.EnsureSuccessStatusCode();

                // Cоздаем JSON объект
                string latestBlockJson = await response.Content.ReadAsStringAsync();
                JObject latestBlock = JObject.Parse(latestBlockJson);

                string blockHash = latestBlock["hash"].ToString();

                // Получаем данные о блоке по его хэшу
                response = await client.GetAsync(blockDataUrl + blockHash);
                response.EnsureSuccessStatusCode();
                string blockDataJson = await response.Content.ReadAsStringAsync();

                //Десериализация из файла json
                blockOfChain deserialized = JsonConvert.DeserializeObject<blockOfChain>(blockDataJson);

                Console.WriteLine("Загружен послейдний блок из блокчейна Bitcoin\n");
                Console.WriteLine("Основые данные: ");
                Console.WriteLine("Хэш:\t" + deserialized.Hash);
                Console.WriteLine("Высота блока:\t" + deserialized.Height);

                int unixTimestamp = deserialized.Time; // Пример Unix временной метки
                System.DateTime epoch = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
                System.DateTime dateTime = epoch.AddSeconds(unixTimestamp);
                Console.WriteLine("Время появления:\t" + dateTime);


                // Сохраняем данные в JSON файл
                System.IO.File.WriteAllText("latestBlock.json", blockDataJson);
                Console.WriteLine("\nДанные последнего блока сохранены в файл latestBlock.json");

                // Парсим данные из JSON в СSV
                // Загружаем файл JSON
                string path = File.ReadAllText("latestBlock.json");

                // Создаем новый файл
                Workbook workbook = new Workbook();

                // Cоздаем новый лист
                Cells cells = workbook.Worksheets[0].Cells;

                // Устанавливаем настройки
                JsonLayoutOptions options = new JsonLayoutOptions();
                options.ConvertNumericOrDate = true;
                options.ArrayAsTable = true;
                options.IgnoreTitle = false;
                
                // Cохраняем файл
                JsonUtility.ImportData(path, cells, 0, 0, options);

                workbook.Save("Output.csv");
                
                Console.WriteLine("Данные последнего блока после парсинга сохранены в файл Output.csv");
                Console.WriteLine("Файл находится в папке проекта");
                Console.ReadKey();

            }
            


        }
    }
    
}



