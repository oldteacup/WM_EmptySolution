namespace GoogleAnalysis
{
    internal static class AnalysisHelpers
    {

        public static string GetDescription(this DataServicesPlatform dsp)
        {
            string value = dsp.ToString();
            System.Reflection.FieldInfo field = dsp.GetType().GetField(value);
            object[] objs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);    //获取描述属性
            if (objs.Length == 0)    //当描述属性没有时，直接返回名称
                return value;
            System.ComponentModel.DescriptionAttribute descriptionAttribute
                = (System.ComponentModel.DescriptionAttribute)objs[0];
            return descriptionAttribute.Description;
        }


        /// <summary>  
        /// 获取本机MAC地址  
        /// </summary>  
        /// <returns>本机MAC地址</returns>  
        public static string MacAddress
        {
            get
            {
                try
                {
                    System.Management.ManagementClass mc = new System.Management.ManagementClass("Win32_NetworkAdapterConfiguration");
                    System.Management.ManagementObjectCollection moc = mc.GetInstances();
                    foreach (System.Management.ManagementObject mo in moc)
                    {
                        if ((bool)mo["IPEnabled"] == true)
                        {
                            return mo["MacAddress"].ToString();
                        }
                    }
                    return string.Empty;
                }
                catch
                {
                    return "unknown mac";
                }
            }
        }
    }
}
