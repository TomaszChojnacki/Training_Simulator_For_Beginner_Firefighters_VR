#define NUM_TEX_COORD_INTERPOLATORS 0
#define NUM_MATERIAL_TEXCOORDS_VERTEX 0
#define NUM_CUSTOM_VERTEX_INTERPOLATORS 0

struct Input
{
	//float3 Normal;
	float2 uv_MainTex : TEXCOORD0;
	float2 uv2_Material_Texture2D_0 : TEXCOORD1;
	float4 color : COLOR;
	float4 tangent;
	//float4 normal;
	float3 viewDir;
	float4 screenPos;
	float3 worldPos;
	//float3 worldNormal;
	float3 normal2;
};
struct SurfaceOutputStandard
{
	float3 Albedo;		// base (diffuse or specular) color
	float3 Normal;		// tangent space normal, if written
	half3 Emission;
	half Metallic;		// 0=non-metal, 1=metal
	// Smoothness is the user facing name, it should be perceptual smoothness but user should not have to deal with it.
	// Everywhere in the code you meet smoothness it is perceptual smoothness
	half Smoothness;	// 0=rough, 1=smooth
	half Occlusion;		// occlusion (default 1)
	float Alpha;		// alpha for transparencies
};

//#define HDRP 1
#define URP 1
#define UE5
//#define HAS_CUSTOMIZED_UVS 1
#define MATERIAL_TANGENTSPACENORMAL 1
//struct Material
//{
	//samplers start
SAMPLER( SamplerState_Linear_Repeat );
SAMPLER( SamplerState_Linear_Clamp );
TEXTURE2D(       Material_Texture2D_0 );
SAMPLER(  samplerMaterial_Texture2D_0 );
float4 Material_Texture2D_0_TexelSize;
float4 Material_Texture2D_0_ST;
TEXTURE2D(       Material_Texture2D_1 );
SAMPLER(  samplerMaterial_Texture2D_1 );
float4 Material_Texture2D_1_TexelSize;
float4 Material_Texture2D_1_ST;
TEXTURE2D(       Material_Texture2D_2 );
SAMPLER(  samplerMaterial_Texture2D_2 );
float4 Material_Texture2D_2_TexelSize;
float4 Material_Texture2D_2_ST;
TEXTURE2D(       Material_Texture2D_3 );
SAMPLER(  samplerMaterial_Texture2D_3 );
float4 Material_Texture2D_3_TexelSize;
float4 Material_Texture2D_3_ST;
TEXTURE2D(       Material_Texture2D_4 );
SAMPLER(  samplerMaterial_Texture2D_4 );
float4 Material_Texture2D_4_TexelSize;
float4 Material_Texture2D_4_ST;
TEXTURE2D(       Material_Texture2D_5 );
SAMPLER(  samplerMaterial_Texture2D_5 );
float4 Material_Texture2D_5_TexelSize;
float4 Material_Texture2D_5_ST;
TEXTURE2D(       Material_Texture2D_6 );
SAMPLER(  samplerMaterial_Texture2D_6 );
float4 Material_Texture2D_6_TexelSize;
float4 Material_Texture2D_6_ST;
TEXTURE2D(       Material_Texture2D_7 );
SAMPLER(  samplerMaterial_Texture2D_7 );
float4 Material_Texture2D_7_TexelSize;
float4 Material_Texture2D_7_ST;
uniform float4 SelectionColor;
uniform float WS_Tilling;
uniform float Normal_Strength;
uniform float Detail_Normal_Tilling;
uniform float Detail_Normal_Strength;
uniform float4 Base_Color_Multiplier;
uniform float Second_BaseColor_Grunge_Tilling;
uniform float Second_BaseColor_Grunge_Strength;
uniform float Metallic_Constant;
uniform float Roughness_Multiplier;
uniform float R_Overall_Grunge_Power;
uniform float Roughness_Grunge_02_Amount;
uniform float Roughness_Grunge_01_Amount;
uniform float Roughness_Grunge_Tilling;
uniform float R_01_Grunge_Power;
uniform float R_02_Grunge_Power;

//};

#ifdef UE5
	#define UE_LWC_RENDER_TILE_SIZE			2097152.0
	#define UE_LWC_RENDER_TILE_SIZE_SQRT	1448.15466
	#define UE_LWC_RENDER_TILE_SIZE_RSQRT	0.000690533954
	#define UE_LWC_RENDER_TILE_SIZE_RCP		4.76837158e-07
	#define UE_LWC_RENDER_TILE_SIZE_FMOD_PI		0.673652053
	#define UE_LWC_RENDER_TILE_SIZE_FMOD_2PI	0.673652053
	#define INVARIANT(X) X
	#define PI 					(3.1415926535897932)

	#include "LargeWorldCoordinates.hlsl"
#endif
struct MaterialStruct
{
	float4 PreshaderBuffer[6];
	float4 ScalarExpressions[1];
	float VTPackedPageTableUniform[2];
	float VTPackedUniform[1];
};
#define SVTPackedUniform VTPackedUniform
static SamplerState View_MaterialTextureBilinearWrapedSampler;
static SamplerState View_MaterialTextureBilinearClampedSampler;
struct ViewStruct
{
	float GameTime;
	float RealTime;
	float DeltaTime;
	float PrevFrameGameTime;
	float PrevFrameRealTime;
	float MaterialTextureMipBias;	
	float4 PrimitiveSceneData[ 40 ];
	float4 TemporalAAParams;
	float2 ViewRectMin;
	float4 ViewSizeAndInvSize;
	float2 ResolutionFractionAndInv;
	float MaterialTextureDerivativeMultiply;
	uint StateFrameIndexMod8;
	uint StateFrameIndex;
	float FrameNumber;
	float2 FieldOfViewWideAngles;
	float4 RuntimeVirtualTextureMipLevel;
	float PreExposure;
	float4 BufferBilinearUVMinMax;
    float OneOverPreExposure;
};
struct ResolvedViewStruct
{
	#ifdef UE5
		FLWCVector3 WorldCameraOrigin;
		FLWCVector3 PrevWorldCameraOrigin;
		FLWCVector3 PreViewTranslation;
		FLWCVector3 WorldViewOrigin;
	#else
		float3 WorldCameraOrigin;
		float3 PrevWorldCameraOrigin;
		float3 PreViewTranslation;
		float3 WorldViewOrigin;
	#endif
	float4 ScreenPositionScaleBias;
	float4x4 TranslatedWorldToView;
	float4x4 TranslatedWorldToCameraView;
	float4x4 TranslatedWorldToClip;
	float4x4 ViewToTranslatedWorld;
	float4x4 PrevViewToTranslatedWorld;
	float4x4 CameraViewToTranslatedWorld;
	float4 BufferBilinearUVMinMax;
	float4 XRPassthroughCameraUVs[ 2 ];
};
struct PrimitiveStruct
{
	float4x4 WorldToLocal;
	float4x4 LocalToWorld;
};

static ViewStruct View;
static ResolvedViewStruct ResolvedView;
static PrimitiveStruct Primitive;
uniform float4 View_BufferSizeAndInvSize;
uniform float4 LocalObjectBoundsMin;
uniform float4 LocalObjectBoundsMax;
static SamplerState Material_Wrap_WorldGroupSettings;
static SamplerState Material_Clamp_WorldGroupSettings;

#include "UnrealCommon.cginc"

static MaterialStruct Material;
void InitializeExpressions()
{
	Material.PreshaderBuffer[0] = float4(-0.002500,0.000000,-0.000839,0.900000);//(Unknown)
	Material.PreshaderBuffer[1] = float4(0.000000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[2] = float4(1.000000,1.000000,1.000000,-0.001429);//(Unknown)
	Material.PreshaderBuffer[3] = float4(0.309452,0.000000,5.000000,-0.001479);//(Unknown)
	Material.PreshaderBuffer[4] = float4(-0.204118,1.236462,0.317083,0.800000);//(Unknown)
	Material.PreshaderBuffer[5] = float4(0.725333,0.000000,0.000000,0.000000);//(Unknown)

	Material.PreshaderBuffer[0].x = rcp( Abs( WS_Tilling ) * -1 );
	Material.PreshaderBuffer[0].y = 1 - Normal_Strength;
	Material.PreshaderBuffer[0].z = rcp( Abs( Detail_Normal_Tilling ) * -1 );
	Material.PreshaderBuffer[0].w = 1 - Detail_Normal_Strength;
	Material.PreshaderBuffer[1].x = SelectionColor.w;
	Material.PreshaderBuffer[1].yzw = SelectionColor.xyz;
	Material.PreshaderBuffer[2].xyz = Base_Color_Multiplier.xyz;
	Material.PreshaderBuffer[2].w = rcp( Abs( Second_BaseColor_Grunge_Tilling ) * -1 );
	Material.PreshaderBuffer[3].x = Second_BaseColor_Grunge_Strength;
	Material.PreshaderBuffer[3].y = Round( Metallic_Constant );
	Material.PreshaderBuffer[3].z = Roughness_Multiplier;
	Material.PreshaderBuffer[3].w = rcp( Abs( Roughness_Grunge_Tilling ) * -1 );
	Material.PreshaderBuffer[4].x = R_01_Grunge_Power;
	Material.PreshaderBuffer[4].y = Roughness_Grunge_01_Amount;
	Material.PreshaderBuffer[4].z = R_02_Grunge_Power;
	Material.PreshaderBuffer[4].w = Roughness_Grunge_02_Amount;
	Material.PreshaderBuffer[5].x = R_Overall_Grunge_Power;
}
float3 GetMaterialWorldPositionOffset(FMaterialVertexParameters Parameters)
{
SHADER_PUSH_WARNINGS_STATE
SHADER_DISABLE_WARNINGS
	return MaterialFloat3(0.00000000,0.00000000,0.00000000);;
SHADER_POP_WARNINGS_STATE
}
void CalcPixelMaterialInputs(in out FMaterialPixelParameters Parameters, in out FPixelMaterialInputs PixelMaterialInputs)
{
	//WorldAligned texturing & others use normals & stuff that think Z is up
	Parameters.TangentToWorld[0] = Parameters.TangentToWorld[0].xzy;
	Parameters.TangentToWorld[1] = Parameters.TangentToWorld[1].xzy;
	Parameters.TangentToWorld[2] = Parameters.TangentToWorld[2].xzy;

	float3 WorldNormalCopy = Parameters.WorldNormal;

SHADER_PUSH_WARNINGS_STATE
SHADER_DISABLE_WARNINGS
	// Initial calculations (required for Normal)
	FWSVector3 Local0 = GetWorldPosition_NoMaterialOffsets(Parameters);
	FWSVector3 Local1 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local0)), WSGetY(DERIV_BASE_VALUE(Local0)), WSGetZ(DERIV_BASE_VALUE(Local0)));
	FWSVector3 Local2 = WSMultiply(DERIV_BASE_VALUE(Local1), ((MaterialFloat3)Material.PreshaderBuffer[0].x));
	FWSVector2 Local3 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local2)), WSGetZ(DERIV_BASE_VALUE(Local2)));
	MaterialFloat2 Local4 = WSApplyAddressMode(DERIV_BASE_VALUE(Local3), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local5 = MaterialStoreTexCoordScale(Parameters, Local4, 5);
	MaterialFloat4 Local6 = UnpackNormalMap(Texture2DSample(Material_Texture2D_0,GetMaterialSharedSampler(samplerMaterial_Texture2D_0,View_MaterialTextureBilinearWrapedSampler),Local4));
	MaterialFloat Local7 = MaterialStoreTexSample(Parameters, Local6, 5);
	FWSVector2 Local8 = MakeWSVector(WSGetY(DERIV_BASE_VALUE(Local2)), WSGetZ(DERIV_BASE_VALUE(Local2)));
	MaterialFloat2 Local9 = WSApplyAddressMode(DERIV_BASE_VALUE(Local8), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local10 = MaterialStoreTexCoordScale(Parameters, Local9, 5);
	MaterialFloat4 Local11 = UnpackNormalMap(Texture2DSample(Material_Texture2D_0,GetMaterialSharedSampler(samplerMaterial_Texture2D_0,View_MaterialTextureBilinearWrapedSampler),Local9));
	MaterialFloat Local12 = MaterialStoreTexSample(Parameters, Local11, 5);
	MaterialFloat3 Local13 = Parameters.TangentToWorld[2];
	MaterialFloat Local14 = DERIV_BASE_VALUE(Local13).r;
	MaterialFloat Local15 = abs(DERIV_BASE_VALUE(Local14));
	MaterialFloat Local16 = lerp((0.00000000 - 1.00000000),(1.00000000 + 1.00000000),DERIV_BASE_VALUE(Local15));
	MaterialFloat Local17 = saturate(DERIV_BASE_VALUE(Local16));
	MaterialFloat Local18 = DERIV_BASE_VALUE(Local17).r;
	MaterialFloat Local19 = DERIV_BASE_VALUE(Local18).r;
	MaterialFloat3 Local20 = lerp(Local6.rgb,Local11.rgb,DERIV_BASE_VALUE(Local19));
	FWSVector2 Local21 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local2)), WSGetY(DERIV_BASE_VALUE(Local2)));
	MaterialFloat2 Local22 = WSApplyAddressMode(DERIV_BASE_VALUE(Local21), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local23 = MaterialStoreTexCoordScale(Parameters, Local22, 5);
	MaterialFloat4 Local24 = UnpackNormalMap(Texture2DSample(Material_Texture2D_0,GetMaterialSharedSampler(samplerMaterial_Texture2D_0,View_MaterialTextureBilinearWrapedSampler),Local22));
	MaterialFloat Local25 = MaterialStoreTexSample(Parameters, Local24, 5);
	MaterialFloat Local26 = DERIV_BASE_VALUE(Local13).b;
	MaterialFloat Local27 = abs(DERIV_BASE_VALUE(Local26));
	MaterialFloat Local28 = lerp((0.00000000 - 1.00000000),(1.00000000 + 1.00000000),DERIV_BASE_VALUE(Local27));
	MaterialFloat Local29 = saturate(DERIV_BASE_VALUE(Local28));
	MaterialFloat Local30 = DERIV_BASE_VALUE(Local29).r;
	MaterialFloat Local31 = DERIV_BASE_VALUE(Local30).r;
	MaterialFloat3 Local32 = lerp(Local20,Local24.rgb,DERIV_BASE_VALUE(Local31));
	MaterialFloat3 Local33 = lerp(Local32,MaterialFloat3(0.00000000,0.00000000,1.00000000).rgb,Material.PreshaderBuffer[0].y);
	MaterialFloat Local34 = (Local33.b + 1.00000000);
	MaterialFloat3 Local35 = normalize(DERIV_BASE_VALUE(Local13));
	MaterialFloat3 Local36 = cross(DERIV_BASE_VALUE(Local35),normalize(MaterialFloat3(0.00000000,0.00000000,1.00000000).rgb));
	MaterialFloat Local37 = dot(DERIV_BASE_VALUE(Local36),DERIV_BASE_VALUE(Local36));
	MaterialFloat3 Local38 = normalize(DERIV_BASE_VALUE(Local36));
	MaterialFloat4 Local39 = MaterialFloat4(DERIV_BASE_VALUE(Local38),0.00000000);
	MaterialFloat4 Local40 = select((abs(DERIV_BASE_VALUE(Local37) - 0.00000100) > 0.00001000), select((DERIV_BASE_VALUE(Local37) >= 0.00000100), DERIV_BASE_VALUE(Local39), MaterialFloat4(MaterialFloat3(0.00000000,0.00000000,0.00000000),1.00000000)), MaterialFloat4(MaterialFloat3(0.00000000,0.00000000,0.00000000),1.00000000));
	MaterialFloat3 Local41 = DERIV_BASE_VALUE(Local40).rgb;
	FWSVector3 Local42 = WSMultiply(DERIV_BASE_VALUE(Local1), ((MaterialFloat3)Material.PreshaderBuffer[0].z));
	FWSVector2 Local43 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local42)), WSGetZ(DERIV_BASE_VALUE(Local42)));
	MaterialFloat2 Local44 = WSApplyAddressMode(DERIV_BASE_VALUE(Local43), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local45 = MaterialStoreTexCoordScale(Parameters, Local44, 10);
	MaterialFloat4 Local46 = UnpackNormalMap(Texture2DSample(Material_Texture2D_1,GetMaterialSharedSampler(samplerMaterial_Texture2D_1,View_MaterialTextureBilinearWrapedSampler),Local44));
	MaterialFloat Local47 = MaterialStoreTexSample(Parameters, Local46, 10);
	MaterialFloat Local48 = dot(DERIV_BASE_VALUE(Local13),MaterialFloat3(0.00000000,1.00000000,0.00000000).rgb);
	MaterialFloat Local49 = select((DERIV_BASE_VALUE(Local48) >= 0.00000000), -1.00000000, 1.00000000);
	MaterialFloat3 Local50 = (Local46.rgb * MaterialFloat3(MaterialFloat2(Local49,-1.00000000),1.00000000));
	FWSVector2 Local51 = MakeWSVector(WSGetY(DERIV_BASE_VALUE(Local42)), WSGetZ(DERIV_BASE_VALUE(Local42)));
	MaterialFloat2 Local52 = WSApplyAddressMode(DERIV_BASE_VALUE(Local51), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local53 = MaterialStoreTexCoordScale(Parameters, Local52, 10);
	MaterialFloat4 Local54 = UnpackNormalMap(Texture2DSample(Material_Texture2D_1,GetMaterialSharedSampler(samplerMaterial_Texture2D_1,View_MaterialTextureBilinearWrapedSampler),Local52));
	MaterialFloat Local55 = MaterialStoreTexSample(Parameters, Local54, 10);
	MaterialFloat Local56 = dot(DERIV_BASE_VALUE(Local13),MaterialFloat3(1.00000000,0.00000000,0.00000000).rgb);
	MaterialFloat Local57 = select((DERIV_BASE_VALUE(Local56) >= 0.00000000), 1.00000000, -1.00000000);
	MaterialFloat3 Local58 = (Local54.rgb * MaterialFloat3(MaterialFloat2(Local57,-1.00000000),1.00000000));
	MaterialFloat3 Local59 = mul(MaterialFloat3(0.00000000,0.00000000,1.00000000), Parameters.TangentToWorld);
	MaterialFloat Local60 = abs(Local59.r);
	MaterialFloat Local61 = lerp((0.00000000 - 0.00000000),(0.00000000 + 1.00000000),DERIV_BASE_VALUE(Local60));
	MaterialFloat Local62 = saturate(DERIV_BASE_VALUE(Local61));
	MaterialFloat Local63 = DERIV_BASE_VALUE(Local62).r;
	MaterialFloat3 Local64 = lerp(Local50,Local58,DERIV_BASE_VALUE(Local63));
	MaterialFloat3 Local65 = (DERIV_BASE_VALUE(Local41) * ((MaterialFloat3)Local64.r));
	MaterialFloat3 Local66 = cross(DERIV_BASE_VALUE(Local36),DERIV_BASE_VALUE(Local35));
	MaterialFloat Local67 = dot(DERIV_BASE_VALUE(Local66),DERIV_BASE_VALUE(Local66));
	MaterialFloat3 Local68 = normalize(DERIV_BASE_VALUE(Local66));
	MaterialFloat4 Local69 = MaterialFloat4(DERIV_BASE_VALUE(Local68),0.00000000);
	MaterialFloat4 Local70 = select((abs(DERIV_BASE_VALUE(Local67) - 0.00000100) > 0.00001000), select((DERIV_BASE_VALUE(Local67) >= 0.00000100), DERIV_BASE_VALUE(Local69), MaterialFloat4(MaterialFloat3(0.00000000,0.00000000,0.00000000),1.00000000)), MaterialFloat4(MaterialFloat3(0.00000000,0.00000000,0.00000000),1.00000000));
	MaterialFloat3 Local71 = DERIV_BASE_VALUE(Local70).rgb;
	MaterialFloat3 Local72 = (DERIV_BASE_VALUE(Local71) * ((MaterialFloat3)Local64.g));
	MaterialFloat3 Local73 = (Local65 + Local72);
	MaterialFloat3 Local74 = (DERIV_BASE_VALUE(Local35) * ((MaterialFloat3)Local64.b));
	MaterialFloat3 Local75 = (Local74 + MaterialFloat3(0.00000000,0.00000000,0.00000000));
	MaterialFloat3 Local76 = (Local73 + Local75);
	MaterialFloat3 Local77 = cross(DERIV_BASE_VALUE(Local35),normalize(MaterialFloat3(0.00000000,1.00000000,0.00000000).rgb));
	MaterialFloat Local78 = dot(DERIV_BASE_VALUE(Local77),DERIV_BASE_VALUE(Local77));
	MaterialFloat3 Local79 = normalize(DERIV_BASE_VALUE(Local77));
	MaterialFloat4 Local80 = MaterialFloat4(DERIV_BASE_VALUE(Local79),0.00000000);
	MaterialFloat4 Local81 = select((abs(DERIV_BASE_VALUE(Local78) - 0.00000100) > 0.00001000), select((DERIV_BASE_VALUE(Local78) >= 0.00000100), DERIV_BASE_VALUE(Local80), MaterialFloat4(MaterialFloat3(0.00000000,0.00000000,0.00000000),1.00000000)), MaterialFloat4(MaterialFloat3(0.00000000,0.00000000,0.00000000),1.00000000));
	MaterialFloat3 Local82 = DERIV_BASE_VALUE(Local81).rgb;
	FWSVector2 Local83 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local42)), WSGetY(DERIV_BASE_VALUE(Local42)));
	MaterialFloat2 Local84 = WSApplyAddressMode(DERIV_BASE_VALUE(Local83), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local85 = MaterialStoreTexCoordScale(Parameters, Local84, 10);
	MaterialFloat4 Local86 = UnpackNormalMap(Texture2DSample(Material_Texture2D_1,GetMaterialSharedSampler(samplerMaterial_Texture2D_1,View_MaterialTextureBilinearWrapedSampler),Local84));
	MaterialFloat Local87 = MaterialStoreTexSample(Parameters, Local86, 10);
	MaterialFloat Local88 = dot(DERIV_BASE_VALUE(Local13),MaterialFloat3(0.00000000,0.00000000,1.00000000).rgb);
	MaterialFloat Local89 = select((DERIV_BASE_VALUE(Local88) >= 0.00000000), 1.00000000, -1.00000000);
	MaterialFloat3 Local90 = (Local86.rgb * MaterialFloat3(MaterialFloat2(Local89,-1.00000000),1.00000000));
	MaterialFloat3 Local91 = (DERIV_BASE_VALUE(Local82) * ((MaterialFloat3)Local90.r));
	MaterialFloat3 Local92 = cross(DERIV_BASE_VALUE(Local77),DERIV_BASE_VALUE(Local35));
	MaterialFloat Local93 = dot(DERIV_BASE_VALUE(Local92),DERIV_BASE_VALUE(Local92));
	MaterialFloat3 Local94 = normalize(DERIV_BASE_VALUE(Local92));
	MaterialFloat4 Local95 = MaterialFloat4(DERIV_BASE_VALUE(Local94),0.00000000);
	MaterialFloat4 Local96 = select((abs(DERIV_BASE_VALUE(Local93) - 0.00000100) > 0.00001000), select((DERIV_BASE_VALUE(Local93) >= 0.00000100), DERIV_BASE_VALUE(Local95), MaterialFloat4(MaterialFloat3(0.00000000,0.00000000,0.00000000),1.00000000)), MaterialFloat4(MaterialFloat3(0.00000000,0.00000000,0.00000000),1.00000000));
	MaterialFloat3 Local97 = DERIV_BASE_VALUE(Local96).rgb;
	MaterialFloat3 Local98 = (DERIV_BASE_VALUE(Local97) * ((MaterialFloat3)Local90.g));
	MaterialFloat3 Local99 = (Local91 + Local98);
	MaterialFloat3 Local100 = (DERIV_BASE_VALUE(Local35) * ((MaterialFloat3)Local90.b));
	MaterialFloat3 Local101 = (Local100 + MaterialFloat3(0.00000000,0.00000000,0.00000000));
	MaterialFloat3 Local102 = (Local99 + Local101);
	MaterialFloat Local103 = abs(Local59.b);
	MaterialFloat Local104 = lerp((0.00000000 - 0.00000000),(0.00000000 + 1.00000000),DERIV_BASE_VALUE(Local103));
	MaterialFloat Local105 = saturate(DERIV_BASE_VALUE(Local104));
	MaterialFloat Local106 = DERIV_BASE_VALUE(Local105).r;
	MaterialFloat3 Local107 = lerp(Local76,Local102,DERIV_BASE_VALUE(Local106));
	MaterialFloat3 Local108 = mul((MaterialFloat3x3)(Parameters.TangentToWorld), Local107);
	MaterialFloat3 Local109 = lerp(Local108,MaterialFloat3(0.00000000,0.00000000,1.00000000).rgb,Material.PreshaderBuffer[0].w);
	MaterialFloat2 Local110 = (Local109.rg * ((MaterialFloat2)-1.00000000));
	MaterialFloat Local111 = dot(MaterialFloat3(Local33.rg,Local34),MaterialFloat3(Local110,Local109.b));
	MaterialFloat3 Local112 = (MaterialFloat3(Local33.rg,Local34) * ((MaterialFloat3)Local111));
	MaterialFloat3 Local113 = (((MaterialFloat3)Local34) * MaterialFloat3(Local110,Local109.b));
	MaterialFloat3 Local114 = (Local112 - Local113);
	MaterialFloat3 Local115 = normalize(Local114);

	// The Normal is a special case as it might have its own expressions and also be used to calculate other inputs, so perform the assignment here
	PixelMaterialInputs.Normal = Local115;

SHADER_POP_WARNINGS_STATE

#if TEMPLATE_USES_SUBSTRATE
	Parameters.SubstratePixelFootprint = SubstrateGetPixelFootprint(Parameters.WorldPosition_CamRelative, GetRoughnessFromNormalCurvature(Parameters));
	Parameters.SharedLocalBases = SubstrateInitialiseSharedLocalBases();
	Parameters.SubstrateTree = GetInitialisedSubstrateTree();
#if SUBSTRATE_USE_FULLYSIMPLIFIED_MATERIAL == 1
	Parameters.SharedLocalBasesFullySimplified = SubstrateInitialiseSharedLocalBases();
	Parameters.SubstrateTreeFullySimplified = GetInitialisedSubstrateTree();
#endif
#endif

	// Note that here MaterialNormal can be in world space or tangent space
	float3 MaterialNormal = GetMaterialNormal(Parameters, PixelMaterialInputs);

#if MATERIAL_TANGENTSPACENORMAL

#if FEATURE_LEVEL >= FEATURE_LEVEL_SM4
	// Mobile will rely on only the final normalize for performance
	MaterialNormal = normalize(MaterialNormal);
#endif

	// normalizing after the tangent space to world space conversion improves quality with sheared bases (UV layout to WS causes shrearing)
	// use full precision normalize to avoid overflows
	Parameters.WorldNormal = TransformTangentNormalToWorld(Parameters.TangentToWorld, MaterialNormal);

#else //MATERIAL_TANGENTSPACENORMAL

	Parameters.WorldNormal = normalize(MaterialNormal);

#endif //MATERIAL_TANGENTSPACENORMAL

#if MATERIAL_TANGENTSPACENORMAL || TWO_SIDED_WORLD_SPACE_SINGLELAYERWATER_NORMAL
	// flip the normal for backfaces being rendered with a two-sided material
	Parameters.WorldNormal *= Parameters.TwoSidedSign;
#endif

	Parameters.ReflectionVector = ReflectionAboutCustomWorldNormal(Parameters, Parameters.WorldNormal, false);

#if !PARTICLE_SPRITE_FACTORY
	Parameters.Particle.MotionBlurFade = 1.0f;
#endif // !PARTICLE_SPRITE_FACTORY

SHADER_PUSH_WARNINGS_STATE
SHADER_DISABLE_WARNINGS
	// Now the rest of the inputs
	MaterialFloat3 Local116 = lerp(MaterialFloat3(0.00000000,0.00000000,0.00000000),Material.PreshaderBuffer[1].yzw,Material.PreshaderBuffer[1].x);
	MaterialFloat Local117 = MaterialStoreTexCoordScale(Parameters, Local4, 3);
	MaterialFloat4 Local118 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_2,GetMaterialSharedSampler(samplerMaterial_Texture2D_2,View_MaterialTextureBilinearWrapedSampler),Local4));
	MaterialFloat Local119 = MaterialStoreTexSample(Parameters, Local118, 3);
	MaterialFloat Local120 = MaterialStoreTexCoordScale(Parameters, Local9, 3);
	MaterialFloat4 Local121 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_2,GetMaterialSharedSampler(samplerMaterial_Texture2D_2,View_MaterialTextureBilinearWrapedSampler),Local9));
	MaterialFloat Local122 = MaterialStoreTexSample(Parameters, Local121, 3);
	MaterialFloat3 Local123 = lerp(Local118.rgb,Local121.rgb,DERIV_BASE_VALUE(Local19));
	MaterialFloat Local124 = MaterialStoreTexCoordScale(Parameters, Local22, 3);
	MaterialFloat4 Local125 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_2,GetMaterialSharedSampler(samplerMaterial_Texture2D_2,View_MaterialTextureBilinearWrapedSampler),Local22));
	MaterialFloat Local126 = MaterialStoreTexSample(Parameters, Local125, 3);
	MaterialFloat3 Local127 = lerp(Local123,Local125.rgb,DERIV_BASE_VALUE(Local31));
	MaterialFloat4 Local128 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_3,GetMaterialSharedSampler(samplerMaterial_Texture2D_3,View_MaterialTextureBilinearWrapedSampler),Local4));
	MaterialFloat Local129 = MaterialStoreTexSample(Parameters, Local128, 3);
	MaterialFloat4 Local130 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_3,GetMaterialSharedSampler(samplerMaterial_Texture2D_3,View_MaterialTextureBilinearWrapedSampler),Local9));
	MaterialFloat Local131 = MaterialStoreTexSample(Parameters, Local130, 3);
	MaterialFloat3 Local132 = lerp(Local128.rgb,Local130.rgb,DERIV_BASE_VALUE(Local19));
	MaterialFloat4 Local133 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_3,GetMaterialSharedSampler(samplerMaterial_Texture2D_3,View_MaterialTextureBilinearWrapedSampler),Local22));
	MaterialFloat Local134 = MaterialStoreTexSample(Parameters, Local133, 3);
	MaterialFloat3 Local135 = lerp(Local132,Local133.rgb,DERIV_BASE_VALUE(Local31));
	MaterialFloat3 Local136 = (Local135 * Material.PreshaderBuffer[2].xyz);
	FWSVector3 Local137 = WSMultiply(DERIV_BASE_VALUE(Local1), ((MaterialFloat3)Material.PreshaderBuffer[2].w));
	FWSVector2 Local138 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local137)), WSGetZ(DERIV_BASE_VALUE(Local137)));
	MaterialFloat2 Local139 = WSApplyAddressMode(DERIV_BASE_VALUE(Local138), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local140 = MaterialStoreTexCoordScale(Parameters, Local139, 9);
	MaterialFloat4 Local141 = ProcessMaterialLinearGreyscaleTextureLookup(Texture2DSample(Material_Texture2D_4,GetMaterialSharedSampler(samplerMaterial_Texture2D_4,View_MaterialTextureBilinearWrapedSampler),Local139));
	MaterialFloat Local142 = MaterialStoreTexSample(Parameters, Local141, 9);
	FWSVector2 Local143 = MakeWSVector(WSGetY(DERIV_BASE_VALUE(Local137)), WSGetZ(DERIV_BASE_VALUE(Local137)));
	MaterialFloat2 Local144 = WSApplyAddressMode(DERIV_BASE_VALUE(Local143), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local145 = MaterialStoreTexCoordScale(Parameters, Local144, 9);
	MaterialFloat4 Local146 = ProcessMaterialLinearGreyscaleTextureLookup(Texture2DSample(Material_Texture2D_4,GetMaterialSharedSampler(samplerMaterial_Texture2D_4,View_MaterialTextureBilinearWrapedSampler),Local144));
	MaterialFloat Local147 = MaterialStoreTexSample(Parameters, Local146, 9);
	MaterialFloat3 Local148 = lerp(Local141.rgb,Local146.rgb,DERIV_BASE_VALUE(Local19));
	FWSVector2 Local149 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local137)), WSGetY(DERIV_BASE_VALUE(Local137)));
	MaterialFloat2 Local150 = WSApplyAddressMode(DERIV_BASE_VALUE(Local149), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local151 = MaterialStoreTexCoordScale(Parameters, Local150, 9);
	MaterialFloat4 Local152 = ProcessMaterialLinearGreyscaleTextureLookup(Texture2DSample(Material_Texture2D_4,GetMaterialSharedSampler(samplerMaterial_Texture2D_4,View_MaterialTextureBilinearWrapedSampler),Local150));
	MaterialFloat Local153 = MaterialStoreTexSample(Parameters, Local152, 9);
	MaterialFloat3 Local154 = lerp(Local148,Local152.rgb,DERIV_BASE_VALUE(Local31));
	MaterialFloat3 Local155 = PositiveClampedPow(Local154,((MaterialFloat3)Material.PreshaderBuffer[3].x));
	MaterialFloat3 Local156 = lerp(Local127,Local136,Local155);
	MaterialFloat Local157 = MaterialStoreTexCoordScale(Parameters, Local4, 4);
	MaterialFloat4 Local158 = ProcessMaterialLinearGreyscaleTextureLookup(Texture2DSample(Material_Texture2D_5,GetMaterialSharedSampler(samplerMaterial_Texture2D_5,View_MaterialTextureBilinearWrapedSampler),Local4));
	MaterialFloat Local159 = MaterialStoreTexSample(Parameters, Local158, 4);
	MaterialFloat Local160 = MaterialStoreTexCoordScale(Parameters, Local9, 4);
	MaterialFloat4 Local161 = ProcessMaterialLinearGreyscaleTextureLookup(Texture2DSample(Material_Texture2D_5,GetMaterialSharedSampler(samplerMaterial_Texture2D_5,View_MaterialTextureBilinearWrapedSampler),Local9));
	MaterialFloat Local162 = MaterialStoreTexSample(Parameters, Local161, 4);
	MaterialFloat3 Local163 = lerp(Local158.rgb,Local161.rgb,DERIV_BASE_VALUE(Local19));
	MaterialFloat Local164 = MaterialStoreTexCoordScale(Parameters, Local22, 4);
	MaterialFloat4 Local165 = ProcessMaterialLinearGreyscaleTextureLookup(Texture2DSample(Material_Texture2D_5,GetMaterialSharedSampler(samplerMaterial_Texture2D_5,View_MaterialTextureBilinearWrapedSampler),Local22));
	MaterialFloat Local166 = MaterialStoreTexSample(Parameters, Local165, 4);
	MaterialFloat3 Local167 = lerp(Local163,Local165.rgb,DERIV_BASE_VALUE(Local31));
	MaterialFloat3 Local168 = (((MaterialFloat3)Material.PreshaderBuffer[3].z) * Local167);
	FWSVector3 Local169 = WSMultiply(DERIV_BASE_VALUE(Local1), ((MaterialFloat3)Material.PreshaderBuffer[3].w));
	FWSVector2 Local170 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local169)), WSGetZ(DERIV_BASE_VALUE(Local169)));
	MaterialFloat2 Local171 = WSApplyAddressMode(DERIV_BASE_VALUE(Local170), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local172 = MaterialStoreTexCoordScale(Parameters, Local171, 7);
	MaterialFloat4 Local173 = ProcessMaterialLinearGreyscaleTextureLookup(Texture2DSample(Material_Texture2D_6,GetMaterialSharedSampler(samplerMaterial_Texture2D_6,View_MaterialTextureBilinearWrapedSampler),Local171));
	MaterialFloat Local174 = MaterialStoreTexSample(Parameters, Local173, 7);
	FWSVector2 Local175 = MakeWSVector(WSGetY(DERIV_BASE_VALUE(Local169)), WSGetZ(DERIV_BASE_VALUE(Local169)));
	MaterialFloat2 Local176 = WSApplyAddressMode(DERIV_BASE_VALUE(Local175), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local177 = MaterialStoreTexCoordScale(Parameters, Local176, 7);
	MaterialFloat4 Local178 = ProcessMaterialLinearGreyscaleTextureLookup(Texture2DSample(Material_Texture2D_6,GetMaterialSharedSampler(samplerMaterial_Texture2D_6,View_MaterialTextureBilinearWrapedSampler),Local176));
	MaterialFloat Local179 = MaterialStoreTexSample(Parameters, Local178, 7);
	MaterialFloat3 Local180 = lerp(Local173.rgb,Local178.rgb,DERIV_BASE_VALUE(Local19));
	FWSVector2 Local181 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local169)), WSGetY(DERIV_BASE_VALUE(Local169)));
	MaterialFloat2 Local182 = WSApplyAddressMode(DERIV_BASE_VALUE(Local181), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local183 = MaterialStoreTexCoordScale(Parameters, Local182, 7);
	MaterialFloat4 Local184 = ProcessMaterialLinearGreyscaleTextureLookup(Texture2DSample(Material_Texture2D_6,GetMaterialSharedSampler(samplerMaterial_Texture2D_6,View_MaterialTextureBilinearWrapedSampler),Local182));
	MaterialFloat Local185 = MaterialStoreTexSample(Parameters, Local184, 7);
	MaterialFloat3 Local186 = lerp(Local180,Local184.rgb,DERIV_BASE_VALUE(Local31));
	MaterialFloat3 Local187 = PositiveClampedPow(Local186,((MaterialFloat3)Material.PreshaderBuffer[4].x));
	MaterialFloat Local188 = lerp(Material.PreshaderBuffer[4].y,1.00000000,Local187.x);
	MaterialFloat Local189 = MaterialStoreTexCoordScale(Parameters, Local171, 8);
	MaterialFloat4 Local190 = ProcessMaterialLinearGreyscaleTextureLookup(Texture2DSample(Material_Texture2D_7,GetMaterialSharedSampler(samplerMaterial_Texture2D_7,View_MaterialTextureBilinearWrapedSampler),Local171));
	MaterialFloat Local191 = MaterialStoreTexSample(Parameters, Local190, 8);
	MaterialFloat Local192 = MaterialStoreTexCoordScale(Parameters, Local176, 8);
	MaterialFloat4 Local193 = ProcessMaterialLinearGreyscaleTextureLookup(Texture2DSample(Material_Texture2D_7,GetMaterialSharedSampler(samplerMaterial_Texture2D_7,View_MaterialTextureBilinearWrapedSampler),Local176));
	MaterialFloat Local194 = MaterialStoreTexSample(Parameters, Local193, 8);
	MaterialFloat3 Local195 = lerp(Local190.rgb,Local193.rgb,DERIV_BASE_VALUE(Local19));
	MaterialFloat Local196 = MaterialStoreTexCoordScale(Parameters, Local182, 8);
	MaterialFloat4 Local197 = ProcessMaterialLinearGreyscaleTextureLookup(Texture2DSample(Material_Texture2D_7,GetMaterialSharedSampler(samplerMaterial_Texture2D_7,View_MaterialTextureBilinearWrapedSampler),Local182));
	MaterialFloat Local198 = MaterialStoreTexSample(Parameters, Local197, 8);
	MaterialFloat3 Local199 = lerp(Local195,Local197.rgb,DERIV_BASE_VALUE(Local31));
	MaterialFloat3 Local200 = PositiveClampedPow(Local199,((MaterialFloat3)Material.PreshaderBuffer[4].z));
	MaterialFloat Local201 = lerp(Material.PreshaderBuffer[4].w,Local188,Local200.x);
	MaterialFloat Local202 = (Material.PreshaderBuffer[5].x * Local201);
	MaterialFloat3 Local203 = (Local168 * ((MaterialFloat3)Local202));

	PixelMaterialInputs.EmissiveColor = Local116;
	PixelMaterialInputs.Opacity = 1.00000000;
	PixelMaterialInputs.OpacityMask = 1.00000000;
	PixelMaterialInputs.BaseColor = Local156;
	PixelMaterialInputs.Metallic = Material.PreshaderBuffer[3].y;
	PixelMaterialInputs.Specular = 0.50000000;
	PixelMaterialInputs.Roughness = Local203;
	PixelMaterialInputs.Anisotropy = 0.00000000;
	PixelMaterialInputs.Normal = Local115;
	PixelMaterialInputs.Tangent = MaterialFloat3(1.00000000,0.00000000,0.00000000);
	PixelMaterialInputs.Subsurface = 0;
	PixelMaterialInputs.AmbientOcclusion = 1.00000000;
	PixelMaterialInputs.Refraction = 0;
	PixelMaterialInputs.PixelDepthOffset = 0.00000000;
	PixelMaterialInputs.ShadingModel = 1;
	PixelMaterialInputs.FrontMaterial = GetInitialisedSubstrateData();
	PixelMaterialInputs.SurfaceThickness = 0.01000000;
	PixelMaterialInputs.Displacement = -1.00000000;

SHADER_POP_WARNINGS_STATE

#if MATERIAL_USES_ANISOTROPY
	Parameters.WorldTangent = CalculateAnisotropyTangent(Parameters, PixelMaterialInputs);
#else
	Parameters.WorldTangent = 0;
#endif
}


#define UnityObjectToWorldDir TransformObjectToWorld

void SetupCommonData( int Parameters_PrimitiveId )
{
	View_MaterialTextureBilinearWrapedSampler = SamplerState_Linear_Repeat;
	View_MaterialTextureBilinearClampedSampler = SamplerState_Linear_Clamp;

	Material_Wrap_WorldGroupSettings = SamplerState_Linear_Repeat;
	Material_Clamp_WorldGroupSettings = SamplerState_Linear_Clamp;

	#ifdef FULLSCREEN_SHADERGRAPH
		View.GameTime = View.RealTime = -_Time.y;// _Time is (t/20, t, t*2, t*3)
	#else
		View.GameTime = View.RealTime = _Time.y;// _Time is (t/20, t, t*2, t*3)
	#endif
	
	View.PrevFrameGameTime = View.GameTime - unity_DeltaTime.x;//(dt, 1/dt, smoothDt, 1/smoothDt)
	View.PrevFrameRealTime = View.RealTime;
	View.DeltaTime = unity_DeltaTime.x;
	View.MaterialTextureMipBias = 0.0;
	View.TemporalAAParams = float4( 0, 0, 0, 0 );
	View.ViewRectMin = float2( 0, 0 );
	View.ViewSizeAndInvSize = View_BufferSizeAndInvSize;
	View.ResolutionFractionAndInv = float2( View_BufferSizeAndInvSize.x / View_BufferSizeAndInvSize.y, 1.0 / ( View_BufferSizeAndInvSize.x / View_BufferSizeAndInvSize.y ));
	View.MaterialTextureDerivativeMultiply = 1.0f;
	View.StateFrameIndexMod8 = 0;
	View.FrameNumber = (int)_Time.y;
	View.FieldOfViewWideAngles = float2( PI * 0.42f, PI * 0.42f );//75degrees, default unity
	View.RuntimeVirtualTextureMipLevel = float4( 0, 0, 0, 0 );
	View.PreExposure = 1;
    View.OneOverPreExposure = 1;
	View.BufferBilinearUVMinMax = float4(
		View_BufferSizeAndInvSize.z * ( 0 + 0.5 ),//EffectiveViewRect.Min.X
		View_BufferSizeAndInvSize.w * ( 0 + 0.5 ),//EffectiveViewRect.Min.Y
		View_BufferSizeAndInvSize.z * ( View_BufferSizeAndInvSize.x - 0.5 ),//EffectiveViewRect.Max.X
		View_BufferSizeAndInvSize.w * ( View_BufferSizeAndInvSize.y - 0.5 ) );//EffectiveViewRect.Max.Y

	for( int i2 = 0; i2 < 40; i2++ )
		View.PrimitiveSceneData[ i2 ] = float4( 0, 0, 0, 0 );

	float4x4 LocalToWorld = transpose( UNITY_MATRIX_M );
    LocalToWorld[3] = float4(ToUnrealPos(LocalToWorld[3]), LocalToWorld[3].w);
	float4x4 WorldToLocal = transpose( UNITY_MATRIX_I_M );
	float4x4 ViewMatrix = transpose( UNITY_MATRIX_V );
	float4x4 InverseViewMatrix = transpose( UNITY_MATRIX_I_V );
	float4x4 ViewProjectionMatrix = transpose( UNITY_MATRIX_VP );
	uint PrimitiveBaseOffset = Parameters_PrimitiveId * PRIMITIVE_SCENE_DATA_STRIDE;
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 0 ] = LocalToWorld[ 0 ];//LocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 1 ] = LocalToWorld[ 1 ];//LocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 2 ] = LocalToWorld[ 2 ];//LocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 3 ] = LocalToWorld[ 3 ];//LocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 5 ] = float4( ToUnrealPos( SHADERGRAPH_OBJECT_POSITION ), 100.0 );//ObjectWorldPosition
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 6 ] = WorldToLocal[ 0 ];//WorldToLocal
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 7 ] = WorldToLocal[ 1 ];//WorldToLocal
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 8 ] = WorldToLocal[ 2 ];//WorldToLocal
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 9 ] = WorldToLocal[ 3 ];//WorldToLocal
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 10 ] = LocalToWorld[ 0 ];//PreviousLocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 11 ] = LocalToWorld[ 1 ];//PreviousLocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 12 ] = LocalToWorld[ 2 ];//PreviousLocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 13 ] = LocalToWorld[ 3 ];//PreviousLocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 18 ] = float4( ToUnrealPos( SHADERGRAPH_OBJECT_POSITION ), 0 );//ActorWorldPosition
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 19 ] = LocalObjectBoundsMax - LocalObjectBoundsMin;//ObjectBounds
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 21 ] = mul( LocalToWorld, float3( 1, 0, 0 ) );
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 23 ] = LocalObjectBoundsMin;//LocalObjectBoundsMin 
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 24 ] = LocalObjectBoundsMax;//LocalObjectBoundsMax

#ifdef UE5
	ResolvedView.WorldCameraOrigin = LWCPromote( ToUnrealPos( _WorldSpaceCameraPos.xyz ) );
	ResolvedView.PreViewTranslation = LWCPromote( float3( 0, 0, 0 ) );
	ResolvedView.WorldViewOrigin = LWCPromote( float3( 0, 0, 0 ) );
#else
	ResolvedView.WorldCameraOrigin = ToUnrealPos( _WorldSpaceCameraPos.xyz );
	ResolvedView.PreViewTranslation = float3( 0, 0, 0 );
	ResolvedView.WorldViewOrigin = float3( 0, 0, 0 );
#endif
	ResolvedView.PrevWorldCameraOrigin = ResolvedView.WorldCameraOrigin;
	ResolvedView.ScreenPositionScaleBias = float4( 1, 1, 0, 0 );
	ResolvedView.TranslatedWorldToView		 = ViewMatrix;
	ResolvedView.TranslatedWorldToCameraView = ViewMatrix;
	ResolvedView.TranslatedWorldToClip		 = ViewProjectionMatrix;
	ResolvedView.ViewToTranslatedWorld		 = InverseViewMatrix;
	ResolvedView.PrevViewToTranslatedWorld = ResolvedView.ViewToTranslatedWorld;
	ResolvedView.CameraViewToTranslatedWorld = InverseViewMatrix;
	ResolvedView.BufferBilinearUVMinMax = View.BufferBilinearUVMinMax;
	Primitive.WorldToLocal = WorldToLocal;
	Primitive.LocalToWorld = LocalToWorld;
}
#define VS_USES_UNREAL_SPACE 1
float3 PrepareAndGetWPO( float4 VertexColor, float3 UnrealWorldPos, float3 UnrealNormal, float4 InTangent,
						 float4 UV0, float4 UV1 )
{
	InitializeExpressions();
	FMaterialVertexParameters Parameters = (FMaterialVertexParameters)0;

	float3 InWorldNormal = UnrealNormal;
	float4 tangentWorld = InTangent;
	tangentWorld.xyz = normalize( tangentWorld.xyz );
	//float3x3 tangentToWorld = CreateTangentToWorldPerVertex( InWorldNormal, tangentWorld.xyz, tangentWorld.w );
	Parameters.TangentToWorld = float3x3( normalize( cross( InWorldNormal, tangentWorld.xyz ) * tangentWorld.w ), tangentWorld.xyz, InWorldNormal );

	
	#ifdef VS_USES_UNREAL_SPACE
		UnrealWorldPos = ToUnrealPos( UnrealWorldPos );
	#endif
	Parameters.WorldPosition = UnrealWorldPos;
	#ifdef VS_USES_UNREAL_SPACE
		Parameters.TangentToWorld[ 0 ] = Parameters.TangentToWorld[ 0 ].xzy;
		Parameters.TangentToWorld[ 1 ] = Parameters.TangentToWorld[ 1 ].xzy;
		Parameters.TangentToWorld[ 2 ] = Parameters.TangentToWorld[ 2 ].xzy;//WorldAligned texturing uses normals that think Z is up
	#endif

	Parameters.VertexColor = VertexColor;

#if NUM_MATERIAL_TEXCOORDS_VERTEX > 0			
	Parameters.TexCoords[ 0 ] = float2( UV0.x, UV0.y );
#endif
#if NUM_MATERIAL_TEXCOORDS_VERTEX > 1
	Parameters.TexCoords[ 1 ] = float2( UV1.x, UV1.y );
#endif
#if NUM_MATERIAL_TEXCOORDS_VERTEX > 2
	for( int i = 2; i < NUM_TEX_COORD_INTERPOLATORS; i++ )
	{
		Parameters.TexCoords[ i ] = float2( UV0.x, UV0.y );
	}
#endif

	Parameters.PrimitiveId = 0;

	SetupCommonData( Parameters.PrimitiveId );

#ifdef UE5
	Parameters.PrevFrameLocalToWorld = MakeLWCMatrix( float3( 0, 0, 0 ), Primitive.LocalToWorld );
#else
	Parameters.PrevFrameLocalToWorld = Primitive.LocalToWorld;
#endif
	
	float3 Offset = float3( 0, 0, 0 );
	Offset = GetMaterialWorldPositionOffset( Parameters );
	#ifdef VS_USES_UNREAL_SPACE
		//Convert from unreal units to unity
		Offset /= float3( 100, 100, 100 );
		Offset = Offset.xzy;
	#endif
	return Offset;
}

void SurfaceReplacement( Input In, out SurfaceOutputStandard o )
{
	InitializeExpressions();

	float3 Z3 = float3( 0, 0, 0 );
	float4 Z4 = float4( 0, 0, 0, 0 );

	float3 UnrealWorldPos = float3( In.worldPos.x, In.worldPos.y, In.worldPos.z );

	float3 UnrealNormal = In.normal2;	

	FMaterialPixelParameters Parameters = (FMaterialPixelParameters)0;
#if NUM_TEX_COORD_INTERPOLATORS > 0
    #ifdef FULLSCREEN_SHADERGRAPH
		Parameters.TexCoords[ 0 ] = float2( In.uv_MainTex.x, In.uv_MainTex.y );
	#else
		Parameters.TexCoords[ 0 ] = float2( In.uv_MainTex.x, 1.0 - In.uv_MainTex.y );
	#endif
#endif
#if NUM_TEX_COORD_INTERPOLATORS > 1
	Parameters.TexCoords[ 1 ] = float2( In.uv2_Material_Texture2D_0.x, 1.0 - In.uv2_Material_Texture2D_0.y );
#endif
#if NUM_TEX_COORD_INTERPOLATORS > 2
	for( int i = 2; i < NUM_TEX_COORD_INTERPOLATORS; i++ )
	{
		Parameters.TexCoords[ i ] = float2( In.uv_MainTex.x, 1.0 - In.uv_MainTex.y );
	}
#endif
	Parameters.PostProcessUV = In.uv_MainTex;
	Parameters.VertexColor = In.color;
	Parameters.WorldNormal = UnrealNormal;
	Parameters.ReflectionVector = half3( 0, 0, 1 );
	//Parameters.CameraVector = normalize( _WorldSpaceCameraPos.xyz - UnrealWorldPos.xyz );
	//Parameters.CameraVector = mul( ( float3x3 )unity_CameraToWorld, float3( 0, 0, 1 ) ) * -1;	
	float3 CameraDirection = (-1 * mul((float3x3)UNITY_MATRIX_M, transpose(mul(UNITY_MATRIX_I_M, UNITY_MATRIX_I_V)) [2].xyz));//From ShaderGraph
	Parameters.CameraVector = CameraDirection;
	Parameters.LightVector = half3( 0, 0, 0 );
	float4 screenpos = In.screenPos;
	screenpos /= screenpos.w;
	Parameters.SvPosition = screenpos;
	Parameters.ScreenPosition = Parameters.SvPosition;

	Parameters.UnMirrored = 1;

	Parameters.TwoSidedSign = 1;


	float3 InWorldNormal = UnrealNormal;	
	float4 tangentWorld = In.tangent;
	tangentWorld.xyz = normalize( tangentWorld.xyz );
	//float3x3 tangentToWorld = CreateTangentToWorldPerVertex( InWorldNormal, tangentWorld.xyz, tangentWorld.w );
	Parameters.TangentToWorld = float3x3( normalize( cross( InWorldNormal, tangentWorld.xyz ) * tangentWorld.w ), tangentWorld.xyz, InWorldNormal );

	//WorldAlignedTexturing in UE relies on the fact that coords there are 100x larger, prepare values for that
	//but watch out for any computation that might get skewed as a side effect
	UnrealWorldPos = ToUnrealPos( UnrealWorldPos );
	
	Parameters.AbsoluteWorldPosition = UnrealWorldPos;
	Parameters.WorldPosition_CamRelative = UnrealWorldPos;
	Parameters.WorldPosition_NoOffsets = UnrealWorldPos;

	Parameters.WorldPosition_NoOffsets_CamRelative = Parameters.WorldPosition_CamRelative;
	Parameters.LightingPositionOffset = float3( 0, 0, 0 );

	Parameters.AOMaterialMask = 0;

	Parameters.Particle.RelativeTime = 0;
	Parameters.Particle.MotionBlurFade;
	Parameters.Particle.Random = 0;
	Parameters.Particle.Velocity = half4( 1, 1, 1, 1 );
	Parameters.Particle.Color = half4( 1, 1, 1, 1 );
	Parameters.Particle.TranslatedWorldPositionAndSize = float4( UnrealWorldPos, 0 );
	Parameters.Particle.MacroUV = half4( 0, 0, 1, 1 );
	Parameters.Particle.DynamicParameter = half4( 0, 0, 0, 0 );
	Parameters.Particle.LocalToWorld = float4x4( Z4, Z4, Z4, Z4 );
	Parameters.Particle.Size = float2( 1, 1 );
	Parameters.Particle.SubUVCoords[ 0 ] = Parameters.Particle.SubUVCoords[ 1 ] = float2( 0, 0 );
	Parameters.Particle.SubUVLerp = 0.0;
	Parameters.TexCoordScalesParams = float2( 0, 0 );
	Parameters.PrimitiveId = 0;
	Parameters.VirtualTextureFeedback = 0;

	FPixelMaterialInputs PixelMaterialInputs = (FPixelMaterialInputs)0;
	PixelMaterialInputs.Normal = float3( 0, 0, 1 );
	PixelMaterialInputs.ShadingModel = 0;
	//PixelMaterialInputs.FrontMaterial = GetStrataUnlitBSDF( float3( 0, 0, 0 ), float3( 0, 0, 0 ) );

	SetupCommonData( Parameters.PrimitiveId );
	//CustomizedUVs
	#if NUM_TEX_COORD_INTERPOLATORS > 0 && HAS_CUSTOMIZED_UVS
		float2 OutTexCoords[ NUM_TEX_COORD_INTERPOLATORS ];
		//Prevent uninitialized reads
		for( int i = 0; i < NUM_TEX_COORD_INTERPOLATORS; i++ )
		{
			OutTexCoords[ i ] = float2( 0, 0 );
		}
		GetMaterialCustomizedUVs( Parameters, OutTexCoords );
		for( int i = 0; i < NUM_TEX_COORD_INTERPOLATORS; i++ )
		{
			Parameters.TexCoords[ i ] = OutTexCoords[ i ];
		}
	#endif
	//<-
	CalcPixelMaterialInputs( Parameters, PixelMaterialInputs );

	#define HAS_WORLDSPACE_NORMAL 0
	#if HAS_WORLDSPACE_NORMAL
		PixelMaterialInputs.Normal = mul( PixelMaterialInputs.Normal, (MaterialFloat3x3)( transpose( Parameters.TangentToWorld ) ) );
	#endif

	o.Albedo = PixelMaterialInputs.BaseColor.rgb;
	o.Alpha = PixelMaterialInputs.Opacity;
	//if( PixelMaterialInputs.OpacityMask < 0.333 ) discard;
	//o.Alpha = PixelMaterialInputs.OpacityMask;

	o.Metallic = PixelMaterialInputs.Metallic;
	o.Smoothness = 1.0 - PixelMaterialInputs.Roughness;
	o.Normal = normalize( PixelMaterialInputs.Normal );
	o.Emission = PixelMaterialInputs.EmissiveColor.rgb;
	o.Occlusion = PixelMaterialInputs.AmbientOcclusion;
}