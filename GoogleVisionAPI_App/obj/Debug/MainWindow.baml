   M S B A M L   `   `   `  ����J  FCoolPhotoSearch, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:GoogleVisionAPI_AppGoogleVisionAPI_App  5   	   $    GoogleVisionAPI_App.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   AGoogleVisionAPI_App!clr-namespace:GoogleVisionAPI_App     5   	   -   5          �Name$  AppMainWindow��5   	    � Title$ Cool Photos Search��5   	   $	��350��5   	   $	��850��5   	    � WindowState$ 	Maximized=�5	   	    � 	Resources 5      .   (GoogleVisionAPI_App.BitmapImageConverter   bitmapImageConverter4   .GoogleVisionAPI_App.BitmapImageVisionConverter  bitmapImageVisionConverter8   2GoogleVisionAPI_App.TextInputToVisibilityConverter " textInputToVisibilityConverter  brushWatermarkBackground �� Color  brushWatermarkForeground  brushWatermarkBorder   BooleanToVisibilityConverter  EntryFieldStyle �� 
TargetType �� Property �� Value " ImageGalleryItemsPanelTemplate 	 ListImageDataTemplate �� Padding	 � ColumnDefinitions
 � RowDefinitions �� BitmapScalingMode �� 	Converter �� IsAsync �� Delay �� Mode �� NotifyOnSourceUpdated �� Triggers P� Property P� Value J� Opacity �� Padding��%�	  &	        &	      &	 &     &	 9     &	 X     &	 v     &	 �     &	 �     &	 D    &		 q    7     5   
   63     5   
   69     5   
   6=   �� 5   
   $ White��6;   �� 5   
   $
 Gray��6;   �� 5   
   $ Indigo��67   �� 5   
   6'   �� 5   
   " �6(   .�5      ��  � ��$ Stretch=�64   �� 5       � ��$ Center=�62   �� 5       � ��$
 20,0q�6'   5      �� 5   
   .R�5      � 6   5      �� 5   
   .R�5      �� $ 5q�6   $��5q�5      
�������$��1q�5       ._�5!      � $
��Left=�6   $	��Top=�5"      	 5#      �� 5$      6-   �� 5%      6-   
 5'      �� 5(      6*   �� 5)      6*   .+�5+      � $��0��6   $��0��5,      $	��210��5-      $	��220��5.      .+�5/      �� $ 5q�6!   $��5q�6-   
��� ���._�51      �� $
 Fant=�52   $   $
��Fill=�53   $   $��Center=�54   $   ��51   $   �� *
ImageObj+8    . $ 10��55      56      57      �� 5I      $��0��6!   $��1��5J   !   .+�5K      �� $
��Left=�5L   $   $	��160��5M   $   ���	#FF998942$
��Bold"�5O   $   $��2q�5P   $   $��Segoe UI Semibold)�5Q   $   $��14(�5R   $   $	��Top=�5S   $   $
��Wrap=�6<   ��5K   $   �� *LuceneRanking+$ TwoWay=� . ��5T      �� 5U   "   " ��6(    5V   &   P� 5W   *    � ��$  ��6B   . �5X   .   ��  � y�$ 	Collapsed=�6K   5Y   +   P� 5Z   *    � �� 6B   Z� . �5[   .   ��  � y�$ 	Collapsed=�6K   5\   +   5^   #   5`      �� 5a      $��Google Vision Labels:��6$   $
��Left=�5b   $   $	��160��5c   $   ���	#FFCDA903$
��Bold"�5e   $   $��2q�5f   $   $��Segoe UI Semibold)�5g   $   $��14(�5h   $   $	��Top=�5i   $   $
��Wrap=�6<   6R   �� 5j      $
��Left=�5k   $   $	��160��5l   $   $	��250��5m   $   ��. $��Visible=�5o   $   ���	#FFFBFBFB$
��Bold"�5q   $   $��5q�5r   $   $��Segoe UI Semibold)�5s   $   $��14(�5t   $   $	��Top=�5u   $   $
��Wrap=�6<   ��5j   $   �� *	ImageText+$ TwoWay=� . 5u   R   5v      �� 5x      $��1��6!   $��0��6.   $��Stretch=�5y   !   $��2��5z   !   
���   �$
 0.70Q�5|   !   $	��410��5}   !   .+�5~      �� 
�������$��60��5�       $��	15,0,15,0q�5�       $ 10q�5�       $��SemiBold"�5�       $��Stretch=�5�       $
��Wrap=�5�       ��5~   $   �� *CleanImageName+5�       5�      5�      5�      5�      5�      .��5�      � 
���   �
 5�   
   �� 5�      $��45 �6   6*   �� 5�      $	��70* �6   6+   �� 5�      $��25 �6   6*   .+�5�   
   ��  �� Orientation$ 
Horizontal=�6   $��Stretch=�5�      
�������$��0��5�      .+�5�      �� $��Enter Search Term��6   $
��Auto��5�      $
��Bold"�5�      $��Center=�5�      $��Center=�5�      $��5q�5�      � 5�      #��[ 6   #��[ 5�      $	��350��6<   $��25��6H   .+�5�      �� $	��5,2q�6   $'��!e.g. flowers, cars, computer etc.��6)   #��[ 5�      $	��350��6S   y�5�      `� 5�       `� 	Converter# [ 6'   .1�5�      ��  �� ElementName$ 	SearchBox��6&    �� Path$ Text.IsEmpty�6>   �� 5�      $ 	SearchBox��6&   $ 	IsFocused�6>   5�      5�      �� 5�      -   5        $��	SearchBox��5�      
������ $	��350��5�      #��[ 5�      6N   5�      �� 5�      -   5         ��Name$ SearchButton��5�      $��Search��5�      $��5q�5�      $��60��5�      $��Center=�5�      60   �� 5�      -   5        $ SearchClear��5�      $��Clear��5�      $��5q�5�      $��60��5�      $��Center=�5�      60   �� 5�      -   5        $��ItemCountLabel��5�      $
��5,15q�5�      $��25��5�      $��Number of Images:��5�      ���	#FFCB1E1E$
��Bold"�6I   �� 5�      -   5        $��
ItemsCount��5�      
������ $��25��5�      $
��5,15q�5�      ���	#FFCB1E1E$
��Bold"�5�   /   ��5�      �� $ lbImageGallery��$ Items.Count�$ OneWay=�5�      �� 5�   
   -   5         ��Name$ lbImageGallery��5�      $��1��5�      $��0��5�      #���  5�      #��[	 5�      $��Disabled=�5�      ��5�      �� ��5�      �� 5�       �� EndPoint$ 0.5,1&�6&    �� 
StartPoint$ 0.5,0&�67   .��5�      �  � Color$ Black��6#   � 5�      $ 	#FF1E2A2F��6#    � Offset$ 1Q�65   5�      5�      � 5�      $��Stretch=�6   
�������$��2��5�      	 5�      �� 5�      $	��98* �6'   65   .+�5�      � -   5          �Name$  pbStatus��5�   B   ! � Minimum$! 0Q�6   " � Maximum$	" 100Q�6*   # � Value$# 0Q�68   $��Stretch=�6T   �� 5�      $��Center=�6e   $��Center=�6�   ��6   �� $ pbStatus��$ Value�$ �� StringFormat$$ {0:0}%��6�   5�      5�      5�      