   M S B A M L   `   `   `  ����@  <SPLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:SPLibSPLib  5      O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5      M ISystem, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089A-clr-namespace:System.IO.Ports;assembly=SystemSystem.IO.Ports 5          SPLib.SPSettingsForm   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      %localclr-namespace:SPLib     5      9System&clr-namespace:System;assembly=mscorlib     5      ?ports-clr-namespace:System.IO.Ports;assembly=System     5      X TXceed.Wpf.Toolkit, Version=2.4.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5	      -   5          �Name$  _this��5
       � WindowStartupLocation$ CenterOwner=�5       � 
ResizeMode$ NoResize=�62    � Title$' !Параметры COM-порта��5       � SizeToContent$ Height=�5
      $	��270��64    � Icon$ ico.ico��6@    � 	Resources 5         	accept_32  	cancel_32  parityValues S� 
MethodName S� 
ObjectType  System.Enum	 S� MethodParameters
 M� TypeName  stopBitsValues��%f  &	        &	 1     &	 b     &	 �     �� 5   
   $��Resources/accept_32.png��6"   �� 5   
   $��Resources/cancel_32.png��6"   S� 5   
   $ 	GetValues��62   "  5      	 5      M� 5      $
 ports:Parity��6   5      S� 5   
   $ 	GetValues��64   "  5      	 5      M� 5      $
 ports:StopBits��6   5      5       J� CommandBindings 5      )  #System.Windows.Input.CommandBinding  5   
   -   5           Command$# local:AppCommands.TestCommand��5      6A     5    
   -   5        $$ local:AppCommands.ApplyCommand��5       6B     5!   
   -   5        $% local:AppCommands.CancelCommand��5!      6C     5"   
   -   5        $- 'local:AppCommands.AvailablePortsCommand��5"      6K   .��5%      �  � RowDefinitions 5&   
   �� 5'      $
��Auto �6   �� 5(      $
��Auto �6   .+�5,   
   � -   5         �Name$ gMain��5,      $��10q�6    � ColumnDefinitions 5-      �� 5.      $	��105 �6#   �� 5/      $��* �6#   �� 50      $��40 �6#    52      �� 53      $��28 �6    �� 54      $��28 �6    �� 55      $��28 �6    �� 56      $��28 �6    �� 57      $��28 �6    �� 58      $��28 �6    �� 59      $��28 �6    .+�5=      �� $��	Порт:��6   �� 5>      $��Скорость:��6   $��1��6*   �� 5?      $��	Биты:��6   $��2��6-   �� 5@      $��Четность:��6   $��3��6-   �� 5A      $��Стоп.биты:��6   $��4��6-   �� 5B      $"��Таймаут чтения:��6   $��5��63   �� 5C      $"��Таймаут записи:��6   $��6��63   �� 5F      -   5         ��Name$ cbPort��5F      $	��0,2q�6-   $��1��6:    �� 
IsEditable .  �� Text 5G      �� *
PortName+ �� Mode$ TwoWay=��� 5I      -   5        $ 
cbBaudRate��5I      $	��0,2q�6-   $��1��6:   $��1��6J    . ��5J      ��    SPLib.SPConnection   	BaudRates �� Source# Z  6^   �� *
BaudRate+$ TwoWay=��� 5K      -	   5        $ 
cbDataBits��5K      $	��0,2q�6-   $��1��6:   $��2��6J   ��5L      ��    DataBits# Z  �� SelectedItem 6]   �� *
DataBits+$ TwoWay=��� 5M      -
   5        $ cbParity��5M      $	��0,2q�6-   $��1��6:   $��3��6J   ��5N      �� # [  6U   �� *Parity+$ TwoWay=��� 5O      -   5        $ 
cbStopBits��5O      $	��0,2q�6-   $��1��6:   $��4��6J   ��5P      �� # [  6W   �� *
StopBits+$ TwoWay=�%  Xceed.Wpf.Toolkit.IntegerUpDown  5Q      -   5          Name$ tbReadTimeout��5Q   !   �  �Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   Minimum$ -1X�68   $��5��6E   $��1��6R   $	��0,2q�6b      Value 5R   !   ��  �� Path$ ReadTimeout�$ TwoWay=�  5S      -   5        $ tbWriteTimeout��5S   !   $ -1X�69   $��6��6F   $��1��6S   $	��0,2q�6c    5T   !   �� $ WriteTimeout�$ TwoWay=��� 5W      -   5         ��Name$ cbIsAvailablePorts��5W      $��2��63   $	��5,0q�6C   $
��Left=�6P   $��Center=�6k    � ToolTip$: 4Показывать только доступные��6�   $-��'local:AppCommands.AvailablePortsCommand��5X      �� 5Z      $��	бит/с��6   $��1��6&   $��2��63   �� 5[      $��мсек��6   $��5��6(   $��2��65   �� 5\      $��мсек��6   $��6��6(   $��2��65   5]      � 5_   
   $��1��6   
������� 5`      �� 5a      $
��Auto �6#   �� 5b      $
��Auto �6#   �� 5c      $��* �6#   .+�5f      �� -   5         ��Name$ bApply��5f      $��40��6%   $��40��61   $% Сохранить (Ctrl+Enter)��6<   $$��local:AppCommands.ApplyCommand��6]   $��10q�5g      #��[  6!   �� 5h      -   5        $ bCancel��5h      $��40��6&   $��40��62   $ Отмена (Esc)��6=   $%��local:AppCommands.CancelCommand��6T   $��
0,10,10,10q�5i      #��[ 6)   $��1��6N   �� 5j      -   5        $ bTest��5j      $��30��6$   $��50��60   $ Отмена (Esc)��6;   $#��local:AppCommands.TestCommand��6R   $��20,10,20,10q�5k      $��Тест��6*   $��Right=�69   $��2��6U   5m         SPLib.CircularProgressBar  5o   
   -   5          Name$ 
pbCircular��5o   $   $y�	Collapsed=�68   5q      5r      