   M S B A M L   `   `   `  ����E  AFileDialog, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:FileDialog
FileDialog  5   	       FileDialog.Webcam   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:FileDialog     5   	     � Title$  Webcam��5   	   $	��450��6   $	��800��6%   .��5	      � .+�5
   
   �� -   5         ��Name$ txtPath��5
      $
��Left=�6#   $��	86,25,0,0q�6>    �� TextWrapping$
 Wrap=�6Q   $�� ��6e   $	��Top=�6m   $	��235��6�    J� RenderTransformOrigin$ 0.529,0.553&�6�   $��22��6�   �� 5   
   $
��Path��6   $
��Left=�6   $��	25,21,0,0q�6:   $	��Top=�6M   �� 5   
   -   5        $��
btnBrowser��5      $��Browser��6#   $
��Left=�65   $��
347,24,0,0q�6P   $	��Top=�6d   6|   �� 5   
   $��Webcam��6   $
��Left=�6!   $��
581,26,0,0q�6<   $	��Top=�6P   �� 5   
   -   5         ��Name$ lsvImge��5      $��25,78,350,0q�6O   $	��315��6d   $	��Top=�6q    �� View 5      �� 5      .P�5      �� $��Image��6%    �� Width$	 100��64    �� CellTemplate 5      �� 5      .R�5   "   �� $��50��6A   $��50��6L   ��6(   �� *Image+6L   5      5      �� 5      $
��Path��6%   $	 150��63    �� DisplayMemberBinding 6?   �� *Path+�� 5      $��	File name��6%   $	 150��68    6D   �� *
FileName+5      	 � ContextMenu	 5      �� 5      .@�5      m� -   5        $
��Open��5      6-   m� 5      -   5        $��Upload��5      6/   m� 5       -   5        $��Delete��5       6/   5!      5#      �� 5$   
   -   5        
 ��Name$
 camImg��5$      $
��Left=�6    $	��312��6;   $��
477,78,0,0q�6H   $	��Top=�6\   $	��264��6t   �� 5%   
   -   5        $��
btnCapture��5%      $��Capture��6#   $
��Left=�65   $��591,362,0,0q�6P   $	��Top=�6e   $ -0.017,0.463&�6}   6�   �� 5&   
   -	   5        $
��Back��5&      $
��Left=�6    $��	750,6,0,0q�6;   $	��Top=�6N   6f   5(      5)      