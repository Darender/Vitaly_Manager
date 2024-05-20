function modificarClientes(idCliente) {
    $.ajax({
        url: '/ControladorClientes/ObtenerCliente', // Ajusta la URL según sea necesario
        type: 'GET',
        data: { id: idCliente },
        success: function (response) {
            console.log(response);

            document.getElementById('validacion00_ID').value = response.id;
            document.getElementById('validacion01_Nombres').value = response.nombres;
            document.getElementById('validacion02_Apellidos').value = response.apellidos;
            document.getElementById('validacion03_Telefonos').value = response.telefono;
            document.getElementById('validacion04_Edades').value = response.edad;

            var selectElement = document.getElementById('validacion05_Genero');

            // Verificar si el elemento existe
            if (response.genero === "Hombre") {
                selectElement.selectedIndex = 1; // Cambia el índice a 1 para seleccionar la opción "Hombre"
            } else if (response.genero === "Mujer") {
                selectElement.selectedIndex = 2;
            } else {
                selectElement.selectedIndex = 3;
            }

            document.getElementById('boton_subir').textContent = "Modificar";
            document.querySelector('.card-title').textContent = 'Modificar datos del cliente';
            
            //document.getElementById('formulario').action = "/ControladorClientes/modificarCliente";
        },
        error: function (error) {
            console.error('Error al obtener los datos del cliente:', error);
        }
    });
}

function eliminarCliente(idCliente) {
    // Eliminar la fila
    var fila = document.getElementById(idCliente);
    if (fila) {
        fila.parentNode.removeChild(fila);

        // Enviar solicitud AJAX al servidor
        $.ajax({
            url: '/ControladorClientes/Eliminar', // Ajusta la URL según sea necesario
            type: 'DELETE',
            data: { id: idCliente },
            success: function (response) {
                document.getElementById('Porcentaje30DiasUsuarios').textContent = "+"+response.porcentaje+"%";
                document.getElementById('CantidadUsuarios').textContent = response.cantidad;

                console.log(response);
            },
            error: function (error) {
                // Manejar errores
                console.error(error);
            }
        });
    }
}