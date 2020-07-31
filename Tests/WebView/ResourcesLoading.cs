﻿using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using WebViewControl;

namespace Tests.WebView
{

    public class ResourcesLoading : WebViewTestBase {

        [Test(Description = "Html load encoding is well handled")]
        public async Task HtmlIsWellEncoded() {
            await Run(async () => {
                const string BodyContent = "some text and a double byte char '●'";

                await Load($"<html><script>;</script><body>{BodyContent}</body></html>");

                var body = TargetView.EvaluateScript<string>("document.body.innerText");
                Assert.AreEqual(BodyContent, body);
            });
        }

        [Test(Description = "Embedded files are correctly loaded")]
        public void EmbeddedFilesLoad() {
            var embeddedResourceUrl = new ResourceUrl(GetType().Assembly, "Resources", "EmbeddedJavascriptFile.js");
            var loadTask = Load($"<html><script src='{embeddedResourceUrl}'></script></html>");
            WaitFor(loadTask);
            var embeddedFileLoaded = TargetView.EvaluateScript<bool>("embeddedFileLoaded");
            Assert.IsTrue(embeddedFileLoaded);
        }

        [Test(Description = "Embedded files with dashes in the filename are correctly loaded")]
        public void EmbeddedFilesWithDashesInFilenameLoad() {
            var embeddedResourceUrl = new ResourceUrl(GetType().Assembly, "Resources", "dash-folder", "EmbeddedJavascriptFile-With-Dashes.js");
            var loadTask = Load($"<html><script src='{embeddedResourceUrl}'></script></html>");
            WaitFor(loadTask);
            var embeddedFileLoaded = TargetView.EvaluateScript<bool>("embeddedFileLoaded");
            Assert.IsTrue(embeddedFileLoaded);
        }

        [Test(Description = "Avalonia resource files are loaded")]
        public void ResourceFile() {
            var embeddedResourceUrl = new ResourceUrl(GetType().Assembly, "Resources", "ResourceJavascriptFile.js");
            var loadTask = Load($"<html><script src='{embeddedResourceUrl}'></script></html>");
            WaitFor(loadTask);
            var resourceFileLoaded = TargetView.EvaluateScript<bool>("resourceFileLoaded");
            Assert.IsTrue(resourceFileLoaded);

            Stream missingResource = null;
            Assert.DoesNotThrow(() => missingResource = ResourcesManager.TryGetResourceWithFullPath(GetType().Assembly, new[] { "Resources", "Missing.txt" }));
            Assert.IsNull(missingResource);
        }
    }
}
