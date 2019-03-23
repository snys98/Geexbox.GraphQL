using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microex.All.Common
{
    public static class Extensions
    {
        public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }
        public static TDest MapTo<TSource, TDest>(this TSource source, TDest dest)
        {
            var fromProps = typeof(TSource).GetProperties().ToList();
            var toProps = typeof(TDest).GetProperties().Where(x => x.CanWrite).ToList();

            foreach (var toProp in toProps)
            {
                if (!toProp.PropertyType.IsSubclassOf(typeof(ValueType)))
                {
                    continue;
                }
                var fromProp = fromProps.FirstOrDefault(x =>
                    x.CanRead &&
                    x.PropertyType == toProp.PropertyType &&
                    x.Name == toProp.Name &&
                    x.PropertyType.IsSubclassOf(typeof(ValueType)));
                if (fromProp == default)
                {
                    if (toProp.GetValue(dest) == default)
                    {
                        throw new InvalidOperationException($"invalid prop of '{toProp.Name}'");
                    }
                    continue;
                }
                toProp.SetValue(dest, fromProp.GetValue(source));
            }

            return dest;
        }
        /// <summary>
        /// 生成随机的枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="random"></param>
        /// <param name="collectionToCover">优先覆盖的枚举值集合</param>
        /// <returns></returns>
        public static T GenerateRandomEnum<T>(this Random random, IList<T> collectionToCover = null)
        {
            var v = Enum.GetValues(typeof(T));
            var value = (T)v.GetValue(random.Next(v.Length));
            if (collectionToCover != null && collectionToCover.Any())
            {
                value = collectionToCover[random.Next(collectionToCover.Count - 1)];
                collectionToCover.Remove(value);
            }
            return value;
        }

        /// <summary>
        /// 生成随机的枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="random"></param>
        /// <param name="collectionToCover">优先覆盖的枚举值集合</param>
        /// <returns></returns>
        public static T GenerateRandomClassEnum<T>(this Random random, IList<T> collectionToCover = null)
        {
            var v = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static).ToList();
            var value = (T)v[random.Next(v.Count)].GetValue(null);
            if (collectionToCover != null && collectionToCover.Any())
            {
                value = collectionToCover[random.Next(collectionToCover.Count - 1)];
                collectionToCover.Remove(value);
            }
            return value;
        }


        public static string LoremIpsum(this Random random, int minWords, int maxWords,
            int minSentences, int maxSentences,
            int numParagraphs)
        {

            var words = new[]{"lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
                "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
                "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat"};

            int numSentences = random.Next(maxSentences - minSentences)
                               + minSentences + 1;
            int numWords = random.Next(maxWords - minWords) + minWords + 1;

            StringBuilder result = new StringBuilder();

            for (int p = 0; p < numParagraphs; p++)
            {
                result.Append("<p>");
                for (int s = 0; s < numSentences; s++)
                {
                    for (int w = 0; w < numWords; w++)
                    {
                        if (w > 0) { result.Append(" "); }
                        result.Append(words[random.Next(words.Length)]);
                    }
                    result.Append(". ");
                }
                result.Append("</p>");
            }

            return result.ToString();
        }

        public static string GenerateRandomIdCardNum(this Random random)
        {
            System.Random rd = new System.Random();
            //随机生成发证地
            string area = random.Next(100000, 999999).ToString();

            //随机出生日期
            DateTime birthday = DateTime.Now;
            birthday = birthday.AddYears(-rd.Next(16, 60));
            birthday = birthday.AddMonths(-rd.Next(0, 12));
            birthday = birthday.AddDays(-rd.Next(0, 31));
            //随机码
            string code = rd.Next(1000, 9999).ToString("####");
            //生成完整身份证号
            string codeNumber = area + birthday.ToString("yyyyMMdd") + code;
            double sum = 0;
            string checkCode = null;
            for (int i = 2; i <= 18; i++)
            {
                sum += int.Parse(codeNumber[18 - i].ToString(), NumberStyles.HexNumber) * (Math.Pow(2, i - 1) % 11);
            }
            string[] checkCodes = { "1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2" };
            checkCode = checkCodes[(int)sum % 11];
            codeNumber = codeNumber.Substring(0, 17) + checkCode;
            //
            return codeNumber;
        }

        /// <summary>
        /// 生成随机的枚举
        /// </summary>
        /// <param name="random"></param>
        /// <param name="max"></param>
        /// <param name="collectionToCover">优先覆盖的值集合,每次执行会从集合里面删除值</param>
        /// <param name="min"></param>
        /// <returns></returns>
        public static int Next(this Random random, int min, int max, List<int> collectionToCover)
        {
            var value = random.Next(min, max);
            if (collectionToCover != null && collectionToCover.Any())
            {
                value = collectionToCover[random.Next(collectionToCover.Count - 1)];
                collectionToCover.Remove(value);
            }
            return value;
        }

        public static IPAddress GetLocalIPv4(this IEnumerable<IPAddress> addressList)
        {
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if ((item.NetworkInterfaceType == NetworkInterfaceType.Ethernet || item.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return ip.Address;
                        }
                    }
                }
            }
            return null;
        }
    }
}
