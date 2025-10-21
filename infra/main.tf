terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }

  required_version = ">= 1.5.0"
}

provider "azurerm" {
  features {}
}

# 1. Resource Group
resource "azurerm_resource_group" "rg" {
  name     = "vollmed-rg"
  location = "East US 2"
}

# 2. Storage Account (Function App requer)
resource "azurerm_storage_account" "sa" {
  name                     = "resumo-mensal-function-1234"
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_service_plan" "plan" {
  name                = "vollmed-func-plan"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  os_type             = "Linux"
  sku_name            = "Y1"
  reserved            = true
}

# 4. Function App
resource "azurerm_windows_function_app" "function" {
  name                       = "vollmed-function-app"
  resource_group_name        = azurerm_resource_group.rg.name
  location                   = azurerm_resource_group.rg.location
  storage_account_name       = azurerm_storage_account.sa.name
  storage_account_access_key = azurerm_storage_account.sa.primary_access_key
  service_plan_id            = azurerm_service_plan.plan.id
  functions_extension_version = "~4"

  app_settings = {
    "FUNCTIONS_WORKER_RUNTIME"         = "dotnet-isolated"
    "AzureWebJobsStorage"             = azurerm_storage_account.sa.primary_connection_string
    "ServiceBusConnection"           = "" # preencha depois ou via secret
    "SqlConnectionString"           = ""
    "AzureCosmosDB_DatabaseName"    = "vollmed"
    "AzureCosmosDB_ContainerName"   = "ResumoMensalConsultas"
    "AzureCosmosDB_ConnectionString" = ""
  }
}
