// Fill out your copyright notice in the Description page of Project Settings.

#include "GalaxyGen.h"
#include "SpawnGalaxy.h"
#include "Star.h" 
#include "GameFramework/Actor.h"



//
const int numofSuns=5;
const int maxX = 100;
const int maxZ = 100;
float lumonsity;

// Sets default values
ASpawnGalaxy::ASpawnGalaxy()
{
 	// Set this actor to call Tick() every frame.  You can turn this off to improve performance if you don't need it.
	//PrimaryActorTick.bCanEverTick = true;
	//UE_LOG(LogTemp, Warning, TEXT("222"));
	UE_LOG(LogTemp, Warning, TEXT("START"));
	for (int i = 0; i < numofSuns;i++)
	{
		SpawnSun();
	}

}

// Called when the game starts or when spawned
void ASpawnGalaxy::BeginPlay()
{
	//UE_LOG(LogTemp, Warning, TEXT("222"));
	Super::BeginPlay();	
}


void ASpawnGalaxy::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime); //Call parent class Tick  

	//GEngine->AddOnScreenDebugMessage(-1, 5.f, FColor::Red, TEXT("This is an on screen message!"));
	
							// Make a location for the new actor to spawn at (300 units above this actor)  
	//FVector NewLocation = GetActorLocation() + FVector(0.f, 0.f, 300.f);

	// Spawn the new actor (Using GetClass() instead of AMySpawner so that if someone derives a new class  
	// from AMySpawner we spawn an instance of that class instead)  
	//ASpawnGalaxy* NewActor = GetWorld()->SpawnActor<ASpawnGalaxy>(GetClass(), NewLocation, FRotator::ZeroRotator);

}

//WELL I WOULD DO A SUMMRY HERE BUT SEING AS C++ WONT ELT ME IM GOING TO RAGE ISNTEAD (also this is for spawning sun)
//ALSO FUCK prototypes
//ALSO FUCK INCLUDE
//ITS PART OF THE CLASS thats why its here WHY DO I HAVE TO TELL YOU THIS C++
void ASpawnGalaxy::SpawnSun()
{
	// quick comment to say whoever thought that this was a good idea is bad
	// what not just have rand.range(x,x)
	float xpos = rand() % maxX;
	float zpos = rand() % maxZ;

	FVector hold =  FVector(0.f, 0.f, 300.f);
	FRotator holdr = FRotator::ZeroRotator;
	/*
	AStar* A1 = FindObject<AStar>(nullptr, TEXT("Star"));
	if (A1 == NULL) {
		UE_LOG(LogTemp, Warning, TEXT("NULL"));
		//GEngine->AddOnScreenDebugMessage(-1, 5.f, FColor::Red, TEXT("Null"));
	}*/
	
	//CreateCloneOfMyActor(A1, hold, holdr);

	//AMySpawner* NewActor = GetWorld()->SpawnActor<AMySpawner>(GetClass(), NewLocation, FRotator::ZeroRotator);
	AStar* NewActor = GetWorld()->SpawnActor<AStar>(spawnStar, hold, FRotator::ZeroRotator);
	
		
	//UWorld* World = AActor::GetWorld();
			
	//World->SpawnActor<AStar>();

}


void ASpawnGalaxy::CreateCloneOfMyActor(class AStar* ExistingActor, FVector SpawnLocation, FRotator SpawnRotation)
{
	UE_LOG(LogTemp, Warning, TEXT("YES"));
	//UWorld* World = ExistingActor->GetWorld();
	//FActorSpawnParameters SpawnParams;
	//SpawnParams.Template = ExistingActor;
	//World->SpawnActor<AStar>(ExistingActor->GetClass(), SpawnLocation, SpawnRotation, SpawnParams);
	
}