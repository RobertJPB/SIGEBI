$pagesDir = "c:\Users\Administrador\source\repos\SIGEBI\SIGEBI.Web\Pages"
$viewsDir = "c:\Users\Administrador\source\repos\SIGEBI\SIGEBI.Web\Views"

function Update-View {
    param($FileRelativePath, $NewModelName)
    $source = Join-Path $pagesDir $FileRelativePath
    $dest = Join-Path $viewsDir $FileRelativePath
    
    $content = Get-Content $source -Raw
    $content = $content -replace '(?m)^@page\r?\n', ''
    $content = $content -replace '(?m)^@model .*$', "@model $NewModelName"
    
    Set-Content -Path $dest -Value $content -Encoding UTF8
}

Update-View "Auth\Login.cshtml" "SIGEBI.Web.Controllers.LoginViewModel"
Update-View "Catalogo\Index.cshtml" "SIGEBI.Web.Controllers.CatalogoViewModel"
Update-View "ListaDeseos\Index.cshtml" "SIGEBI.Web.Controllers.ListaDeseosViewModel"
Update-View "Notificaciones\Index.cshtml" "SIGEBI.Web.Controllers.NotificacionesIndexViewModel"
Update-View "Perfil\Index.cshtml" "SIGEBI.Web.Controllers.PerfilIndexViewModel"
Update-View "Prestamos\Index.cshtml" "SIGEBI.Web.Controllers.PrestamosIndexViewModel"

# Move _ViewImports
Copy-Item "$pagesDir\_ViewImports.cshtml" "$viewsDir\_ViewImports.cshtml" -Force
$imports = Get-Content "$viewsDir\_ViewImports.cshtml" -Raw
$imports = $imports -replace '@addTagHelper \*, Microsoft.AspNetCore.Mvc.TagHelpers', "@using SIGEBI.Web.Models`n@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers"
Set-Content -Path "$viewsDir\_ViewImports.cshtml" -Value $imports -Encoding UTF8

Write-Host "Done copying and converting views."
