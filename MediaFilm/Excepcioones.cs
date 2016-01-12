using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MediaFilm 
{
    class DirectoryNotFoundException : Exception
    {
        public DirectoryNotFoundException()
        {

        }

        public DirectoryNotFoundException(string message) : base(message)
        {
            MessageBox.Show(message);
        }

        public DirectoryNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DirectoryNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
    class TooManySerieCoincidencesException : Exception
    {
        public TooManySerieCoincidencesException()
        {
        }

        public TooManySerieCoincidencesException(string message) : base(message)
        {
        }

        public TooManySerieCoincidencesException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TooManySerieCoincidencesException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
    class TipoArchivoNoSoportadoException : Exception
    {
        public TipoArchivoNoSoportadoException()
        {
        }

        public TipoArchivoNoSoportadoException(string message) : base(message)
        {
        }

        public TipoArchivoNoSoportadoException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TipoArchivoNoSoportadoException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
