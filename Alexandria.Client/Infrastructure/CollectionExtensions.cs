using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Alexandria.Messages;
using System.Linq;

namespace Alexandria.Client.Infrastructure
{
    using Caliburn.Core.Invocation;
    using ViewModels;

    public static class CollectionExtensions
	{
		public static void UpdateFrom(this ObservableCollection<BookModel> collection, IEnumerable<BookDTO> mergeSource)
		{
		    Execute.OnUIThreadAsync(() =>{
		        foreach(var bookDTO in mergeSource)
		        {
		            var bookModel = collection.FirstOrDefault(model => model.Id == bookDTO.Id);
		            if(bookModel == null)
		            {
		                bookModel = new BookModel
		                {
		                    Id = bookDTO.Id
		                };
		                collection.Add(bookModel);
		            }
		            MergeValues(bookModel, bookDTO);
		        }
		        var toRemove = collection.Where(orig => mergeSource.Any(merged => merged.Id == orig.Id) == false).ToArray();
		        foreach(var model in toRemove)
		        {
		            collection.Remove(model);
		        }
		    });
		}

        private static void MergeValues(BookModel bookModel, BookDTO bookDTO)
        {
            bookModel.Author = bookDTO.Author;
            bookModel.Name = bookDTO.Name;

        	bookModel.Image = GetImageSource(bookDTO.Image);
        }

    	private static BitmapImage GetImageSource(byte[] image)
    	{
    		var bi = new BitmapImage();
    		bi.BeginInit();
    		bi.StreamSource = new MemoryStream(image);
    		bi.EndInit();
    		return bi;
    	}
	}
}