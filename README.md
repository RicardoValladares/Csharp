# Impresora de Ticket

Pequeño ejemplo hecho en C# Net Framework 4.5 para los que quieren imprimir facturas en postick

![Captura de Pantalla](https://raw.githubusercontent.com/RicardoValladares/Impresora_de_Ticket/main/ticket.png)

```cs

Ticket ticket = new Ticket();
ticket.HeaderImage = picturebox1.Image;
ticket.AddHeaderLine("Super Mercado");
ticket.AddSubHeaderLine("Venta 1");
ticket.AddItem("5","Peras","1.99");
ticket.AddItem("1", "Aguacate", "1.00");
ticket.AddTotal("TOTAL","2.99");
ticket.AddFooterLine("Gracias por su preferencia...");
//ticket.PrintTicket("80mm Series Printer"); especificando nombre de la impresora
ticket.PrintTicket(); //impresora primaria por defecto

```
