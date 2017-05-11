using MovieDX.Forms.CustomControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MovieDX.Forms.Helpers
{
    public class MovieTemplateSelector : Xamarin.Forms.DataTemplateSelector
    {
        readonly DataTemplate MovieCard;
        //readonly DataTemplate MovieCard2;

        public MovieTemplateSelector()
        {
            this.MovieCard= new DataTemplate(typeof(MovieItem));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var movieData = item as MovieDX.Core.Models.DetailedMovie;
            
            if (movieData== null)
                return null;
            DataTemplate selectedTemplate = MovieCard;
                        
            //if (eventData.isPast || eventData.IsCurrentUserOwner)
            //    selectedTemplate = EventCard;
            //else
            //    selectedTemplate = EventCardInvitation;

            return selectedTemplate;
        }
    }
}
