function cargarProductos() {
	return new Promise((resolve, reject) => {
		var tipoSeleccionado = document.getElementById("formulario_tipo").value;

		// Realiza una petición AJAX para obtener los productos del tipo seleccionado
		fetch('/ControladorInventario/ObtenerProductosPorTipo?tipo=' + tipoSeleccionado)
			.then(response => response.json())
			.then(data => {
				var selectProductos = document.getElementById("formulario_producto");
				selectProductos.innerHTML = '<option selected disabled value="" id="formulario_prodcuto_vacio">Por favor seleccione</option>';

				data.forEach(producto => {
					var option = document.createElement("option");
					option.text = producto.nombre + " - " + producto.proveedor;
					option.value = producto.id;
					selectProductos.appendChild(option);
				});

				resolve();
			})
			.catch(error => {
				console.error('Error al obtener productos:', error);
				reject(error);
			});
	});
}