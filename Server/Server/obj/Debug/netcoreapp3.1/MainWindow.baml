   M S B A M L   `   `   `  ????A  =Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ServerServer  5   	       Server.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:Server     5   	     ? Title$  
MainWindow??5   	   $	??450??6   $	??600??6)   .??5	      ?  ? ColumnDefinitions 5
   
   ?? 5      $	??20* ?6   ?? 5      $
??100* ?6   ?? 5      $
??100* ?6   .+?5   
   ?? -   5        $??Start Server??5      $??Center=?6(   $??	0,217,0,0q?6E   $	??Top=?6X   $	??157??6p   $??58??6|   $??2??6?   ?? 5   
   -   5        $??receive_msg??5      $
??Left=?6%   $	??300??6@   $??	50,78,0,0q?6M    ?? TextWrapping$
 Wrap=?6`    ?? 
IsReadOnly$
 True??6t   $	??Top=?6?   $	??246??6?   $??3??6?   ?? 5   
   $??Message??6   $??20(?6"   $
??Left=?60   $??62.2,34,0,0q?6K   $	??Top=?6`   $??1??6x   ?? 5   
   -   5        $??	host_port??5      $??2??6#   $??Center=?63   $??0,52,0,0q?6P   $
 Wrap=?6b   $
??port??6v   $	??Top=?6?   $	??136??6?   $??47??6?   ?? 5   
   -   5        $??host_ip??5      $??2??6!   $??Center=?61   $??	0,131,0,0q?6N   $
 Wrap=?6a   $??ip??6u   $	??Top=?6   $	??136??6?   $??52??6?   ?? 5   
   -   5        $??2??5      $??Button??6!   $??Center=?62   $??	0,336,0,0q?6O   $	??Top=?6b   $??65??6z   $	??157??6?   6?   5      5      