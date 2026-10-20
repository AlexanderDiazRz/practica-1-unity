#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public static class ConstructorNivel2
{
    private const string MarcaConstruccion = "Nivel2_Generado_v6";
    private const string RutaNivel2 = "Assets/Scenes/Nivel2.unity";
    private const string RutaSample = "Assets/Scenes/SampleScene.unity";
    private const string RutaSpikeHead =
        "Assets/Sprites/Nivel2/SpikeHead.png";
    private const string RutaProyectil =
        "Assets/Sprites/Nivel2/DustParticle.png";
    private const string RutaPinchos =
        "Assets/Sprites/Nivel2/Spikes.png";

    [InitializeOnLoadMethod]
    private static void EjecutarUnaVezAlCompilar()
    {
        EditorApplication.delayCall += () =>
        {
            Scene escena = SceneManager.GetActiveScene();

            if (escena.IsValid()
                && string.Equals(
                    escena.name,
                    "Nivel2",
                    StringComparison.OrdinalIgnoreCase
                )
                && Buscar(MarcaConstruccion) == null)
            {
                Construir();
            }
        };
    }

    [MenuItem("Tools/Practica 5/Construir Nivel 2 _F8")]
    public static void Construir()
    {
        Scene escena = SceneManager.GetActiveScene();

        if (!escena.IsValid()
            || !string.Equals(
                escena.name,
                "Nivel2",
                StringComparison.OrdinalIgnoreCase
            ))
        {
            EditorUtility.DisplayDialog(
                "Practica 5",
                "Abre la escena Nivel2 antes de usar esta herramienta.",
                "OK"
            );

            return;
        }

        ConfigurarSprite(RutaSpikeHead, 32f);
        ConfigurarSprite(RutaProyectil, 16f);
        ConfigurarSprite(RutaPinchos, 16f);

        GameObject anterior = Buscar("Nivel2_Generado");

        if (anterior != null)
        {
            UnityEngine.Object.DestroyImmediate(anterior);
        }

        foreach (string marcaAnterior in new[]
        {
            "Nivel2_Generado_v2",
            "Nivel2_Generado_v3",
            "Nivel2_Generado_v4",
            "Nivel2_Generado_v5",
            MarcaConstruccion
        })
        {
            anterior = Buscar(marcaAnterior);

            if (anterior != null)
            {
                UnityEngine.Object.DestroyImmediate(anterior);
            }
        }

        GameObject raiz = new GameObject(MarcaConstruccion);

        Transform jugador = Buscar("personaje")?.transform;

        if (jugador == null)
        {
            EditorUtility.DisplayDialog(
                "Practica 5",
                "No se encontró el objeto personaje en Nivel2.",
                "OK"
            );

            return;
        }

        Tilemap suelo = PrepararTilemap();
        PintarMapa(suelo);

        Canvas canvas = CrearInterfaz(
            raiz.transform,
            out TextMeshProUGUI poderTexto,
            out TextMeshProUGUI vidaJefeTexto,
            out TextMeshProUGUI mensajePuerta,
            out GameObject panelVictoria
        );

        MovimientoPersonaje movimiento =
            jugador.GetComponent<MovimientoPersonaje>();

        jugador.position = new Vector3(2f, 1.4f, 0f);

        movimiento.fuerzaSalto = 9.5f;
        movimiento.velocidadMovimiento = 5f;

        Rigidbody2D cuerpoJugador =
            jugador.GetComponent<Rigidbody2D>();

        if (cuerpoJugador != null)
        {
            cuerpoJugador.gravityScale = 2f;
        }

        PoderNivel2 poder =
            ObtenerOAgregar<PoderNivel2>(jugador.gameObject);

        poder.frutasNecesarias = 5;
        poder.spriteProyectil =
            AssetDatabase.LoadAssetAtPath<Sprite>(RutaProyectil);
        poder.textoPoder = poderTexto;

        PrepararFrutas();
        PrepararEnemigos(raiz.transform);
        PrepararLimites(raiz.transform);
        PrepararFondo(jugador);
        PrepararCamara(jugador);
        PrepararMensajesExistentes();

        GameObject gestorObjeto =
            new GameObject("GestorNivel2");

        gestorObjeto.transform.SetParent(raiz.transform);

        GestorNivel2 gestor =
            gestorObjeto.AddComponent<GestorNivel2>();

        gestor.panelVictoria = panelVictoria;
        gestor.jugador = movimiento;
        gestor.segundosParaReiniciar = 4f;
        gestor.escenaInicial = "SampleScene";

        PrepararPuerta(mensajePuerta);
        CrearJefe(
            raiz.transform,
            gestor,
            vidaJefeTexto
        );

        CrearPinchos(raiz.transform);

        canvas.gameObject.SetActive(true);

        AgregarEscenasAlBuild();

        EditorSceneManager.MarkSceneDirty(escena);
        EditorSceneManager.SaveScene(escena);

        Selection.activeGameObject = raiz;

        EditorUtility.DisplayDialog(
            "Practica 5",
            "Nivel2 fue construido y guardado. Presiona Play para probarlo.",
            "Listo"
        );
    }

    private static Tilemap PrepararTilemap()
    {
        GameObject gridObjeto = Buscar("Grid");

        if (gridObjeto == null)
        {
            gridObjeto = new GameObject("Grid");
            gridObjeto.AddComponent<Grid>();
        }

        GameObject sueloObjeto = Buscar("Suelo");

        if (sueloObjeto == null)
        {
            sueloObjeto = new GameObject("Suelo");
            sueloObjeto.transform.SetParent(gridObjeto.transform);
        }

        Tilemap tilemap =
            ObtenerOAgregar<Tilemap>(sueloObjeto);

        TilemapRenderer renderer =
            ObtenerOAgregar<TilemapRenderer>(sueloObjeto);

        renderer.sortingOrder = 0;

        Rigidbody2D rb =
            ObtenerOAgregar<Rigidbody2D>(sueloObjeto);

        rb.bodyType = RigidbodyType2D.Static;
        rb.simulated = true;

        CompositeCollider2D composite =
            ObtenerOAgregar<CompositeCollider2D>(sueloObjeto);

        composite.geometryType =
            CompositeCollider2D.GeometryType.Polygons;

        TilemapCollider2D collider =
            ObtenerOAgregar<TilemapCollider2D>(sueloObjeto);

        collider.compositeOperation =
            Collider2D.CompositeOperation.Merge;

        tilemap.ClearAllTiles();

        return tilemap;
    }

    private static void PintarMapa(Tilemap tilemap)
    {
        TileBase pasto =
            AssetDatabase.LoadAssetAtPath<TileBase>(
                "Assets/Sprites/terrain/Terrain (16x16)_6.asset"
            );

        TileBase tierra =
            AssetDatabase.LoadAssetAtPath<TileBase>(
                "Assets/Sprites/terrain/Terrain (16x16)_23.asset"
            );

        if (pasto == null || tierra == null)
        {
            throw new InvalidOperationException(
                "No se encontraron los tiles de terreno."
            );
        }

        Suelo(tilemap, pasto, tierra, -3, 17, 0, 6);
        Plataforma(tilemap, pasto, tierra, 6, 11, 2);

        Suelo(tilemap, pasto, tierra, 18, 36, 0, 6);
        Plataforma(tilemap, pasto, tierra, 18, 21, 2);
        Plataforma(tilemap, pasto, tierra, 23, 26, 4);
        Plataforma(tilemap, pasto, tierra, 28, 32, 6);

        Suelo(tilemap, pasto, tierra, 37, 41, 0, 6);
        Suelo(tilemap, pasto, tierra, 47, 52, 0, 6);
        Suelo(tilemap, pasto, tierra, 59, 66, 0, 6);

        Plataforma(tilemap, pasto, tierra, 41, 45, 2);
        Plataforma(tilemap, pasto, tierra, 48, 53, 4);
        Plataforma(tilemap, pasto, tierra, 55, 60, 2);

        Suelo(tilemap, pasto, tierra, 67, 108, 0, 6);

        Plataforma(tilemap, pasto, tierra, 69, 73, 2);
        Plataforma(tilemap, pasto, tierra, 75, 79, 4);
        Plataforma(tilemap, pasto, tierra, 81, 85, 2);
        Plataforma(tilemap, pasto, tierra, 96, 101, 4);

        for (int y = 1; y <= 11; y++)
        {
            tilemap.SetTile(
                new Vector3Int(108, y, 0),
                tierra
            );
        }
    }

    private static void Suelo(
        Tilemap mapa,
        TileBase pasto,
        TileBase tierra,
        int desdeX,
        int hastaX,
        int superficieY,
        int profundidad
    )
    {
        for (int x = desdeX; x <= hastaX; x++)
        {
            mapa.SetTile(
                new Vector3Int(x, superficieY, 0),
                pasto
            );

            for (
                int y = superficieY - 1;
                y >= superficieY - profundidad;
                y--
            )
            {
                mapa.SetTile(
                    new Vector3Int(x, y, 0),
                    tierra
                );
            }
        }
    }

    private static void Plataforma(
        Tilemap mapa,
        TileBase pasto,
        TileBase tierra,
        int desdeX,
        int hastaX,
        int y
    )
    {
        for (int x = desdeX; x <= hastaX; x++)
        {
            mapa.SetTile(
                new Vector3Int(x, y, 0),
                pasto
            );

            mapa.SetTile(
                new Vector3Int(x, y - 1, 0),
                tierra
            );
        }
    }

    private static void PrepararFrutas()
    {
        Colocar("Apple", 9f, 3.3f);
        Colocar("Bananas", 30f, 7.3f);
        Colocar("Cherries", 51f, 5.3f);
        Colocar("Pineapple", 58f, 3.3f);
        Colocar("strawberry", 83f, 3.3f);
    }

    private static void PrepararEnemigos(Transform padre)
    {
        GameObject simple1 =
            Colocar("EnemigoSimple", 14f, 1.3f);

        GameObject simple2 =
            Colocar("EnemigoSimple2", 39f, 1.3f);

        GameObject complejo1 =
            Colocar("EnemigoComplejo", 64f, 1.3f);

        GameObject complejo2 =
            Colocar("EnemigoComplejo2", 79f, 1.3f);

        DuplicarEnemigo(
            simple1,
            "EnemigoExtra_1",
            new Vector3(20f, 3.3f, 0f),
            padre
        );

        DuplicarEnemigo(
            simple2,
            "EnemigoExtra_2",
            new Vector3(44f, 3.3f, 0f),
            padre
        );

        DuplicarEnemigo(
            complejo1,
            "EnemigoExtra_3",
            new Vector3(71f, 3.3f, 0f),
            padre
        );

        DuplicarEnemigo(
            complejo2,
            "EnemigoExtra_4",
            new Vector3(84f, 3.3f, 0f),
            padre
        );
    }

    private static void DuplicarEnemigo(
        GameObject original,
        string nombre,
        Vector3 posicion,
        Transform padre
    )
    {
        if (original == null) return;

        GameObject copia =
            UnityEngine.Object.Instantiate(
                original,
                posicion,
                Quaternion.identity,
                padre
            );

        copia.name = nombre;
        copia.SetActive(true);
    }

    private static void PrepararCamara(Transform jugador)
    {
        Camera camara = Camera.main;

        if (camara == null)
        {
            camara =
                UnityEngine.Object.FindAnyObjectByType<Camera>();
        }

        if (camara == null) return;

        camara.orthographic = true;
        camara.orthographicSize = 8.5f;

        CameraController controlador =
            ObtenerOAgregar<CameraController>(
                camara.gameObject
            );

        controlador.objetive = jugador;
        controlador.cameraSpeed = 0.08f;
        controlador.movement =
            new Vector3(0f, 3.1f, -10f);
        controlador.verticalFollow = 0.15f;
        controlador.verticalDeadZone = 1.5f;

        camara.transform.position =
            jugador.position + controlador.movement;
    }

    private static void PrepararFondo(Transform jugador)
    {
        GameObject fondo = Buscar("Fondo");
        Camera camara = Camera.main;

        if (fondo == null || camara == null) return;

        fondo.transform.position =
            new Vector3(52f, 8f, 0f);

        fondo.transform.localScale = Vector3.one;

        SpriteRenderer renderer =
            fondo.GetComponent<SpriteRenderer>();

        if (renderer != null)
        {
            renderer.drawMode = SpriteDrawMode.Tiled;
            renderer.size = new Vector2(150f, 40f);
            renderer.sortingOrder = -20;
        }

        ParallaxScript parallax =
            ObtenerOAgregar<ParallaxScript>(fondo);

        parallax.cameraTransform = camara.transform;
        parallax.parallaxMultiplier =
            new Vector2(0.15f, 0.05f);
    }

    private static void PrepararLimites(Transform padre)
    {
        GameObject izquierda = Buscar("ParedIzquierda");

        if (izquierda != null)
        {
            izquierda.transform.position =
                new Vector3(-4f, 5f, 0f);

            izquierda.transform.localScale =
                new Vector3(1f, 24f, 1f);
        }

        GameObject derecha = Buscar("Paredderecha");

        if (derecha != null)
        {
            derecha.transform.position =
                new Vector3(109f, 5f, 0f);

            derecha.transform.localScale =
                new Vector3(1f, 24f, 1f);
        }

        GameObject muerte =
            new GameObject("ZonaMuerte");

        muerte.transform.SetParent(padre);
        muerte.transform.position =
            new Vector3(52f, -7f, 0f);

        BoxCollider2D col =
            muerte.AddComponent<BoxCollider2D>();

        col.isTrigger = true;
        col.size = new Vector2(125f, 2f);

        muerte.AddComponent<ZonaPeligrosa>();
    }

    private static Canvas CrearInterfaz(
        Transform padre,
        out TextMeshProUGUI poder,
        out TextMeshProUGUI vidaJefe,
        out TextMeshProUGUI mensajePuerta,
        out GameObject panelVictoria
    )
    {
        GameObject objetoCanvas = new GameObject(
            "UI_Nivel2",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)
        );

        objetoCanvas.transform.SetParent(padre);

        Canvas canvas =
            objetoCanvas.GetComponent<Canvas>();

        canvas.renderMode =
            RenderMode.ScreenSpaceOverlay;

        canvas.sortingOrder = 50;

        CanvasScaler scaler =
            objetoCanvas.GetComponent<CanvasScaler>();

        scaler.uiScaleMode =
            CanvasScaler.ScaleMode.ScaleWithScreenSize;

        scaler.referenceResolution =
            new Vector2(1920f, 1080f);

        poder = CrearTexto(
            objetoCanvas.transform,
            "TextoPoder",
            "Poder: 0/5",
            32,
            new Vector2(1f, 1f),
            new Vector2(-35f, -30f),
            new Vector2(450f, 70f),
            TextAlignmentOptions.TopRight
        );

        poder.rectTransform.pivot =
            new Vector2(1f, 1f);

        vidaJefe = CrearTexto(
            objetoCanvas.transform,
            "TextoVidaJefe",
            "Jefe: 10/10",
            32,
            new Vector2(1f, 1f),
            new Vector2(-35f, -90f),
            new Vector2(450f, 70f),
            TextAlignmentOptions.TopRight
        );

        vidaJefe.rectTransform.pivot =
            new Vector2(1f, 1f);

        vidaJefe.color =
            new Color(1f, 0.35f, 0.35f);

        mensajePuerta = CrearTexto(
            objetoCanvas.transform,
            "MensajePuertaJefe",
            "Necesitas las 5 frutas",
            36,
            new Vector2(0.5f, 0.25f),
            Vector2.zero,
            new Vector2(900f, 90f),
            TextAlignmentOptions.Center
        );

        mensajePuerta.gameObject.SetActive(false);

        panelVictoria = new GameObject(
            "PanelVictoria",
            typeof(RectTransform),
            typeof(Image)
        );

        panelVictoria.transform.SetParent(
            objetoCanvas.transform,
            false
        );

        RectTransform panelRect =
            panelVictoria.GetComponent<RectTransform>();

        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        panelVictoria.GetComponent<Image>().color =
            new Color(0f, 0f, 0f, 0.78f);

        TextMeshProUGUI victoria = CrearTexto(
            panelVictoria.transform,
            "TextoVictoria",
            "¡JUEGO COMPLETADO!\n" +
            "Reiniciando desde el nivel 1...",
            64,
            new Vector2(0.5f, 0.5f),
            Vector2.zero,
            new Vector2(1200f, 300f),
            TextAlignmentOptions.Center
        );

        victoria.color = Color.yellow;

        panelVictoria.SetActive(false);

        return canvas;
    }

    private static TextMeshProUGUI CrearTexto(
        Transform padre,
        string nombre,
        string contenido,
        float tamano,
        Vector2 ancla,
        Vector2 posicion,
        Vector2 medidas,
        TextAlignmentOptions alineacion
    )
    {
        GameObject objeto = new GameObject(
            nombre,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(TextMeshProUGUI)
        );

        objeto.transform.SetParent(padre, false);

        TextMeshProUGUI texto =
            objeto.GetComponent<TextMeshProUGUI>();

        texto.text = contenido;
        texto.fontSize = tamano;
        texto.alignment = alineacion;
        texto.color = Color.white;

        RectTransform rect = texto.rectTransform;

        rect.anchorMin = ancla;
        rect.anchorMax = ancla;
        rect.anchoredPosition = posicion;
        rect.sizeDelta = medidas;

        return texto;
    }

    private static void PrepararPuerta(
        TextMeshProUGUI mensaje
    )
    {
        GameObject puerta = Buscar("puerta");

        if (puerta == null) return;

        Puerta vieja = puerta.GetComponent<Puerta>();

        if (vieja != null)
        {
            UnityEngine.Object.DestroyImmediate(vieja);
        }

        puerta.transform.position =
            new Vector3(88f, 3.2f, 0f);

        puerta.transform.localScale =
            new Vector3(1f, 6f, 1f);

        puerta.SetActive(true);

        BoxCollider2D col =
            ObtenerOAgregar<BoxCollider2D>(puerta);

        col.isTrigger = false;

        PuertaJefe script =
            ObtenerOAgregar<PuertaJefe>(puerta);

        script.frutasNecesarias = 5;
        script.mensaje = mensaje;
    }

    private static void CrearJefe(
        Transform padre,
        GestorNivel2 gestor,
        TextMeshProUGUI textoVida
    )
    {
        GameObject jefe =
            new GameObject("JefeFinal_SpikeHead");

        jefe.transform.SetParent(padre);

        jefe.transform.position =
            new Vector3(100f, 2f, 0f);

        jefe.transform.localScale =
            Vector3.one * 1.8f;

        SpriteRenderer renderer =
            jefe.AddComponent<SpriteRenderer>();

        renderer.sprite =
            AssetDatabase.LoadAssetAtPath<Sprite>(
                RutaSpikeHead
            );

        renderer.sortingOrder = 10;

        Rigidbody2D rb =
            jefe.AddComponent<Rigidbody2D>();

        rb.gravityScale = 3f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode =
            CollisionDetectionMode2D.Continuous;

        BoxCollider2D col =
            jefe.AddComponent<BoxCollider2D>();

        col.size = new Vector2(1.35f, 1.35f);

        int capaEnemigos =
            LayerMask.NameToLayer("Enemies");

        if (capaEnemigos >= 0)
        {
            jefe.layer = capaEnemigos;
        }

        try
        {
            jefe.tag = "Enemy";
        }
        catch
        {
            // El tag Enemy debe existir en Unity.
        }

        JefeFinal script =
            jefe.AddComponent<JefeFinal>();

        script.vidaMaxima = 10;
        script.velocidadPersecucion = 4f;
        script.limiteIzquierdo = 92f;
        script.limiteDerecho = 105f;
        script.textoVida = textoVida;
        script.gestorNivel = gestor;
    }

    private static void CrearPinchos(Transform padre)
    {
        Sprite sprite =
            AssetDatabase.LoadAssetAtPath<Sprite>(
                RutaPinchos
            );

        float[] posiciones =
        {
            16f,
            35f,
            69f,
            74f,
            80f
        };

        foreach (float x in posiciones)
        {
            GameObject trampa =
                new GameObject("Pinchos_" + x);

            trampa.transform.SetParent(padre);

            trampa.transform.position =
                new Vector3(x, 0.9f, 0f);

            trampa.transform.localScale =
                new Vector3(1.5f, 1.5f, 1f);

            SpriteRenderer visual =
                trampa.AddComponent<SpriteRenderer>();

            visual.sprite = sprite;
            visual.sortingOrder = 5;

            BoxCollider2D col =
                trampa.AddComponent<BoxCollider2D>();

            col.isTrigger = true;
            col.size = new Vector2(0.9f, 0.35f);

            trampa.AddComponent<ZonaPeligrosa>();
        }
    }

    private static void PrepararMensajesExistentes()
    {
        foreach (
            TextMeshProUGUI texto
            in ObjetosDeEscena<TextMeshProUGUI>()
        )
        {
            if (
                texto.text != null
                && texto.text.Contains("10 frutas")
            )
            {
                texto.text =
                    "¡Recoge las 5 frutas para activar " +
                    "tu poder y enfrentar al jefe!";

                texto.fontSize = 34f;

                texto.rectTransform.sizeDelta =
                    new Vector2(900f, 180f);
            }
        }
    }

    private static void AgregarEscenasAlBuild()
    {
        List<EditorBuildSettingsScene> escenas =
            EditorBuildSettings.scenes.ToList();

        AgregarSiFalta(escenas, RutaSample);
        AgregarSiFalta(escenas, RutaNivel2);

        EditorBuildSettings.scenes =
            escenas.ToArray();
    }

    private static void AgregarSiFalta(
        List<EditorBuildSettingsScene> escenas,
        string ruta
    )
    {
        if (
            escenas.All(
                e => !string.Equals(
                    e.path,
                    ruta,
                    StringComparison.OrdinalIgnoreCase
                )
            )
        )
        {
            escenas.Add(
                new EditorBuildSettingsScene(
                    ruta,
                    true
                )
            );
        }
    }

    private static void ConfigurarSprite(
        string ruta,
        float ppu
    )
    {
        TextureImporter importer =
            AssetImporter.GetAtPath(ruta)
            as TextureImporter;

        if (importer == null) return;

        importer.textureType =
            TextureImporterType.Sprite;

        importer.spriteImportMode =
            SpriteImportMode.Single;

        importer.spritePixelsPerUnit = ppu;
        importer.filterMode = FilterMode.Point;

        importer.textureCompression =
            TextureImporterCompression.Uncompressed;

        importer.SaveAndReimport();
    }

    private static GameObject Colocar(
        string nombre,
        float x,
        float y
    )
    {
        GameObject objeto = Buscar(nombre);

        if (objeto != null)
        {
            objeto.transform.position =
                new Vector3(x, y, 0f);

            objeto.SetActive(true);
        }

        return objeto;
    }

    private static GameObject Buscar(string nombre)
    {
        Scene escena =
            SceneManager.GetActiveScene();

        return Resources
            .FindObjectsOfTypeAll<GameObject>()
            .FirstOrDefault(
                g =>
                    g.scene == escena
                    && string.Equals(
                        g.name,
                        nombre,
                        StringComparison.OrdinalIgnoreCase
                    )
            );
    }

    private static IEnumerable<T> ObjetosDeEscena<T>()
        where T : Component
    {
        Scene escena =
            SceneManager.GetActiveScene();

        return Resources
            .FindObjectsOfTypeAll<T>()
            .Where(
                c => c.gameObject.scene == escena
            );
    }

    private static T ObtenerOAgregar<T>(
        GameObject objeto
    )
        where T : Component
    {
        T componente = objeto.GetComponent<T>();

        return componente != null
            ? componente
            : objeto.AddComponent<T>();
    }
}

#endif