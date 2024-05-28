function modificarClientes(idCliente) {
    $.ajax({
        url: '/ControladorClientes/ObtenerCliente', 
        type: 'GET',
        data: { id: idCliente },
        success: function (response) {
            document.getElementById('validacion00_ID').value = response.id;
            document.getElementById('validacion01_Nombres').value = response.nombres;
            document.getElementById('validacion02_Apellidos').value = response.apellidos;
            document.getElementById('validacion03_Telefonos').value = response.telefono;
            document.getElementById('validacion04_Edades').value = response.edad;

            var selectElement = document.getElementById('validacion05_Genero');

            // Verificar si el elemento existe
            if (response.genero === "Hombre") {
                selectElement.selectedIndex = 1; 
            } else if (response.genero === "Mujer") {
                selectElement.selectedIndex = 2;
            } else {
                selectElement.selectedIndex = 3;
            }

            document.getElementById('boton_subir').textContent = "Modificar";
            document.querySelector('.card-title').textContent = 'Modificar datos del cliente';
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
                recargarTablaNotificaciones();
                console.log(response);
            },
            error: function (error) {
                // Manejar errores
                console.error(error);
            }
        });
    }
}

function recargarTablaNotificaciones() {
    // Selecciona el elemento ul
    var ul = document.getElementById('tablaNotificaciones');

    // Elimina todo el contenido del ul
    ul.innerHTML = "";

    while (ul.firstChild) {
        ul.removeChild(ul.firstChild);
    }

    $.get('/ControladorClientes/ObtenerListaMovimientos', function (data) {
        // Iterar sobre los datos y construir las filas de la tabla
        data.forEach(function (movimiento) {
            console.log(movimiento);
            // Crea un nuevo elemento li para cada movimiento
            var li = document.createElement('li');
            li.className = 'timeline-panel';

            // Crea el contenido HTML dentro del li
            li.innerHTML = `
                    <div class="media me-2">
                        <img alt="image" width="50" src="/imagenes/usuarios/imagen_${movimiento.usuario_id}.jpg">
                    </div>
                    <div class="media-body">
                        <h5 class="mb-1">${movimiento.titulo}</h5>
                        <small class="d-block">${new Date(movimiento.ingreso).toLocaleString()}</small>
                    </div>
                    <div class="dropdown">
                        <button type="button" class="btn btn-primary light sharp" data-bs-toggle="dropdown">
                            <svg width="18px" height="18px" viewBox="0 0 24 24" version="1.1"><g stroke="none" stroke-width="1" fill="none" fill-rule="evenodd"><rect x="0" y="0" width="24" height="24" /><circle fill="#000000" cx="5" cy="12" r="2" /><circle fill="#000000" cx="12" cy="12" r="2" /><circle fill="#000000" cx="19" cy="12" r="2" /></g></svg>
                        </button>
                        <div class="dropdown-menu dropdown-menu-end">
                            <a class="dropdown-item" onclick="showSweetAlert('${movimiento.titulo}', '${movimiento.descripcion}')" href="#">Detalles</a>
                            <a class="dropdown-item" href="#">Eliminar</a>
                        </div>
                    </div>
                `;

            // Agrega el li al ul
            ul.appendChild(li);
        });
    });
}
