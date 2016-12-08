param($installPath, $toolsPath, $package, $project)

foreach ($reference in $project.Object.References)
{
	switch -regex ($reference.Name.ToLowerInvariant())
	{
		"^standaloneundo$"
		{
			$reference.CopyLocal = $true;
		}
		"^microsoft.visualstudio.coreutility$"
		{
			$reference.CopyLocal = $true;
		}
		"^microsoft.visualstudio.language.intellisense$"
		{
			$reference.CopyLocal = $true;
		}
		"^microsoft.visualstudio.language.standardclassification$"
		{
			$reference.CopyLocal = $true;
		}
		"^microsoft.visualstudio.platform.vseditor$"
		{
			$reference.CopyLocal = $true;
		}
		"^microsoft.visualstudio.platform.vseditor.interop$"
		{
			$reference.EmbedInteropTypes = $false;
			$reference.CopyLocal = $true;
		}
		"^microsoft.visualstudio.text.data$"
		{
			$reference.CopyLocal = $true;
		}
		"^microsoft.visualstudio.text.internal$"
		{
			$reference.CopyLocal = $true;
		}
		"^microsoft.visualstudio.text.logic$"
		{
			$reference.CopyLocal = $true;
		}
		"^microsoft.visualstudio.text.ui$"
		{
			$reference.CopyLocal = $true;
		}
		"^microsoft.visualstudio.text.ui.wpf$"
		{
			$reference.CopyLocal = $true;
		}
		"^microsoft.visualstudio.ui.text.wpf.keyprocessor.implementation$"
		{
			$reference.CopyLocal = $true;
		}
		default
		{
			# ignore
		}
	}
}
