using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Social.UI.Models
{
    //Resim Yülemesi İçin Oluşturulan Class
    public class Upload
    {
        /*  ImageUpload Parametre
        - List<IFormFile> => Form sayfasından gelen fileleri alma 
        - IHostingEnvironment => Proje içerinde resim gideceği konum için
        - out bool result => Eğerki metot başarılı şekilde çalışır ise Out ile Bool değer dönme
        ---------------------------------------------------------------------------------------
        Not: List<IFormFile> => Liste olması birden fazla file gelme ihtimali içindir
        */
        public static string ImageUpload(List<IFormFile> files, IHostingEnvironment env, out bool result)
        {
            /*  ImageUpload
            - Gelen resimlerin wwwroot içerisinde hangi klasöre gideceğini belirleme (path = Path.Combine(env.WebRootPath, "uploads"))
            - Metoda gelen file arasında dönme (List<IFormFile> files)
            - Gelen filenin yolu (path) arasında İmage kontrolu (Resim Dışında Başka dosya olmaması için)
            - Gelen dosyanın 2 mb dan fazla olmaması konrolu (file.Length <= 2097152)
            - File ile gelen resime Guid ile özel bir isim ataması (guidname)
            - Guid name başına wwwroot içerisine gideceği yeri ekleme (filepath)
            - FileStream Using metodu içerisinde kullanarak işlem bittiğinde Ramdan temizlemesini kapatmasını sağlamak
            - FileStream ile resmin yolunu belirterek oraya kopyalşanmasını sağlamak (file.CopyTo(fs);)
            - Geriye Db ye kayıt için resmin yolu ile beraber ismini dönme (return filepath)
            - Olumsuzluklara göre hata mesajı dönme
            */
            result = false;

            var path = Path.Combine(env.WebRootPath, "uploads");

            foreach (var file in files)
            {
                if (file.ContentType.Contains("image"))
                {
                    if (file.Length <= 2097152)
                    {
                        /* guidname
                        - New anahtar kelimesi ile yeni Guid oluşturma
                        - Replace ederek Guid içerisindeki '-' leri '_' çevirme
                        - ToLower ile metinsel harfleri bütük harf yapma
                        - file.ContentType.Split ile resimin pcden gelen yolundan(Path) ".jpeg,jpg,png,img vs" alma Guidin sonuna ekleme
                        */
                        string guidname = $"{Guid.NewGuid().ToString().Replace("-", "_").ToLower()}.{file.ContentType.Split("/")[1]}";

                        var filepath = Path.Combine(path, guidname);

                        using (var fs = new FileStream(filepath, FileMode.Create))
                        {
                            file.CopyTo(fs);
                            result = true;
                            return filepath.Substring(filepath.IndexOf("\\uploads\\"));
                        }
                    }
                    else
                    {
                        return "2 mb dan yüksek resim yükleyemezsiniz!";
                    }
                }
                else
                {
                    return "Lütfen sadece resim dosyası seçin";
                }
            }
            return "Dosya bulunamadı";
        }

    }
}
