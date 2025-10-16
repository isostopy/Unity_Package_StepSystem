
# Step System
Paquete con el que generar cadenas de pasos que van a ocurrir uno detrás de otro. Útil para construir experiencias lineales completas o simplemente pequeñas secuencias predefinidas de eventos dentro de una experiencia más grande.

# Setup
Hay que añadir a la escena un game object con el componente ``StepChain``. Y añadir como hijos de ese, otros game object con los componentes de tipo ``Step`` que queremaos. ¡Y listo! Al ejecutar la cadena se ejecutarán todos los pasos, uno detrás de otro, en el orden en el que aparecen en la jerarquía.

Un mismo objeto puede llevar varios componentes de tipo ``Step`` y también se ejecutarán en el orden en el que están en el inspector.

Modificar la cadena de pasos es tan fácil como añadir nuevos objetos con un componente ``Step`` o reordenar game objects en el editor.

El paquete incluye algunos pasos por defecto como:
|  |  |
|--|--|
| ``GameObjectStep`` | Activa o desactiva un game object. |
| ``AnimationStep`` | Pone una animación de un Animator. |
| ``WaitStep`` | Espera un tiempo antes de continuar con la cadena. |
| ``WaitForButtonStep`` | Espera a que un usuario pulse un botón para continuar. |

# Crear tipos de Step
Para crear experiencias personalizadas seguramente hará falta crear tipos de pasos que resuelvan las necesidades únicas de cada proyecto. Para eso:

* Crea una nueva clase que herede de ``Step``.
* Sobrescribe las funciones siguientes:
	|   |   |
	|---|---|
	| ``OnActivate()`` | Para ejecutar tu código al activar este paso. Es donde normalmente se hace lo que sea que haga este paso. |
	| ``OnEnd()`` | Para ejecutar código al terminar. |
	| ``OnRestart()`` | Para ejecutar código al reiniciar. Esta función debe dejar todo como estaba antes de activar el paso. |

* Llama a la función ``End()`` para terminar el paso y que la cadena pase al siguiente. Si no lo haces, la cadena se atascará en tu paso. 

## Ejemplos
Paso sencillo que imprime un mensaje por consola y termina directamente.
```C#
	public class DebugStep : Step
	{
		protected override void OnActivate()
		{
			Debug.Log("Hello World!", this);
			End();
		}
	}
```
Paso que espera a que ocurra algo para continuar con la cadena. No se llama a ``End()`` directamente despues de activarlo, si no cuando ha ocurrido el evento al que estamos esperando.
```C#
	public class WaitForSomethingStep : Step
	{
		protected override void OnActivate()
		{
			Debug.Log("Paso activado.");
		}

		public void Something()
		{
			End();
		}

		protected override void OnEnd()
		{
			Debug.Log("Paso terminado.");
		}
	}
```
Paso que activa un objeto y lo deja como estaba al reiniciar.
```C#
	public class ActivateObjectStep : Step
	{
		public GameObject targetObject;

		private void Start()
		{
			targetObject.SetActive(false);
		}

		protected override void OnActivate()
		{
			targetObject.SetActive(true);
			End();
		}

		protected override void OnRestart()
		{
			targetObject.SetActive(false);
		}
	}
```
