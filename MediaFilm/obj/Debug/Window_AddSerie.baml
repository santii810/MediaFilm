   M S B A M L   `   `   `  ����D  @MediaFilm, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:MediaFilm	MediaFilm  5   	       MediaFilm.Window_AddSerie   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:MediaFilm     5   	     � Title$  Añadir serie��5   	   $��266.125��6   $��349.625��6/   .��5	      �  J� RenderTransformOrigin$ 0.968,0.489&�6   $��0,0,2,-5q�6/    � ColumnDefinitions 5
   
   �� 5      $
��346* �6   .+�5   
   �� -   5         ��Name$ label��5      $��Nombre��6   $
��Left=�60   $��40��6K   $��	14,13,0,0q�6W   $	��Top=�6j   $	��132��6�   �� 5   
   -   5         ��Name$ textBox_NombreSerie��5      $
��Left=�6/   $��35��6J   $��
146,13,0,0q�6V    �� TextWrapping$
 Wrap=�6j   $	��Top=�6~   $	��183��6�   6�   �� 5   
   -   5        $ 
label_Copy��5      $��	Extension��6$   $
��Left=�68   $��40��6S   $��	14,53,0,0q�6_   $	��Top=�6r   $	��132��6�   �� 5   
   -   5        $ label_Copy1��5      $��
Temporadas��6%   $
��Left=�6:   $��40��6U   $��	14,93,0,0q�6a   $	��Top=�6t   $	��132��6�   �� 5   
   -   5        $ label_Copy2��5      $��Capitulos temporada��6%   $
��Left=�6C   $��40��6^   $��
14,133,0,0q�6j   $	��Top=�6~   $	��132��6�   �� 5   
   -   5         ��Name$ comboBox��5      $
��Left=�6%   $��30��6@   $��
146,58,0,0q�6L   $	��Top=�6`   $	��183��6x   �� 5   
    �� Orientation$ 
Horizontal=�6   $��146,98,11,112q�6.   .+�5      �� -   5        $ textBoxTemporadas��5      $��5,0,0,0q�6K   $	��115��6\   $��0��6h   6q   �� 5      -   5         ��Name$ temporadasUp��5      $��5,0,0,0q�6E   $��˄��6V   $��20��6b   6m   �� 5      -	   5        $ temporadasDown��5      $��˅��6H   $��20��6T   6_   5      �� 5   
   $ 
Horizontal=�6   $��146,132,16,77q�6.   .+�5      �� -
   5        $ textBoxCapitulos��5      $��5,0,0,0q�6J   $	��115��6[   $��0��6g   6p   �� 5      -   5        $ capitulosUp��5      $��5,0,0,0q�6D   $��˄��6U   $��20��6a   6l   �� 5      -   5        $ capitulosDown��5      $��˅��6G   $��20��6S   6^   5      �� 5   
   -   5        $ button_AddSerie��5      $��Añadir serie��6*   $
��Left=�6A   $��32��6\   $��146,168,0,0q�6h   $	��Top=�6}   $	��178��6�   6�   5      5       