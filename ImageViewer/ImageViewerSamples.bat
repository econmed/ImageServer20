:: ImageViewerVolume specific postbuild step

call "..\..\..\..\ImageViewer\ImageViewer.bat" %1

echo Executing ImageViewerSamples post-build step

copy "..\..\..\..\Desktop\Applets\WebBrowser\bin\%1\ClearCanvas.Desktop.Applets.WebBrowser.dll" ".\plugins"
copy "..\..\..\..\Desktop\Applets\WebBrowser\View\WinForms\bin\%1\ClearCanvas.Desktop.Applets.WebBrowser.View.WinForms.dll" ".\plugins"

copy "..\..\..\..\ImageViewer\Tools\ImageProcessing\Filter\bin\%1\ClearCanvas.ImageViewer.Tools.ImageProcessing.Filter.dll" ".\plugins"
copy "..\..\..\..\ImageViewer\Tools\ImageProcessing\Filter\View\WinForms\bin\%1\ClearCanvas.ImageViewer.Tools.ImageProcessing.Filter.View.WinForms.dll" ".\plugins"
