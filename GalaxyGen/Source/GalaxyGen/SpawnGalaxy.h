// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "GameFramework/Actor.h"
#include "SpawnGalaxy.generated.h"




UCLASS()
class GALAXYGEN_API ASpawnGalaxy : public AActor
{
	GENERATED_BODY()
		
	void SpawnSun();
	void CreateCloneOfMyActor(class AStar* ExistingActor, FVector SpawnLocation, FRotator SpawnRotation);
	
public:	
	// Sets default values for this actor's properties
	ASpawnGalaxy();

	// Called when the game starts or when spawned
	virtual void BeginPlay() override;
	
	// Called every frame
	virtual void Tick( float DeltaSeconds ) override;

	
	
};
