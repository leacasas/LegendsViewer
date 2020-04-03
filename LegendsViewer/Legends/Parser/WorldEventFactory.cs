using LegendsViewer.Legends.EventCollections;
using LegendsViewer.Legends.Events;
using System.Collections.Generic;

namespace LegendsViewer.Legends.Parser
{
    internal class WorldEventFactory
    {
        internal static void AddEventToWorld(string type, World world, List<Property> properties)
        {
            switch (type)
            {
                case "add hf entity link":
                    world.Events.Add(new AddHfEntityLink(properties, world));
                    break;
                case "add hf hf link":
                    world.Events.Add(new AddHfhfLink(properties, world));
                    break;
                case "attacked site":
                    world.Events.Add(new AttackedSite(properties, world));
                    break;
                case "body abused":
                    world.Events.Add(new BodyAbused(properties, world));
                    break;
                case "change hf job":
                    world.Events.Add(new ChangeHfJob(properties, world));
                    break;
                case "change hf state":
                    world.Events.Add(new ChangeHfState(properties, world));
                    break;
                case "changed creature type":
                    world.Events.Add(new ChangedCreatureType(properties, world));
                    break;
                case "create entity position":
                    world.Events.Add(new CreateEntityPosition(properties, world));
                    break;
                case "created site":
                    world.Events.Add(new CreatedSite(properties, world));
                    break;
                case "created world construction":
                    world.Events.Add(new CreatedWorldConstruction(properties, world));
                    break;
                case "creature devoured":
                    world.Events.Add(new CreatureDevoured(properties, world));
                    break;
                case "destroyed site":
                    world.Events.Add(new DestroyedSite(properties, world));
                    break;
                case "field battle":
                    world.Events.Add(new FieldBattle(properties, world));
                    break;
                case "hf abducted":
                    world.Events.Add(new HfAbducted(properties, world));
                    break;
                case "hf died":
                    world.Events.Add(new HfDied(properties, world));
                    break;
                case "hf new pet":
                    world.Events.Add(new HfNewPet(properties, world));
                    break;
                case "hf reunion":
                    world.Events.Add(new HfReunion(properties, world));
                    break;
                case "hf simple battle event":
                    world.Events.Add(new HfSimpleBattleEvent(properties, world));
                    break;
                case "hf travel":
                    world.Events.Add(new HfTravel(properties, world));
                    break;
                case "hf wounded":
                    world.Events.Add(new HfWounded(properties, world));
                    break;
                case "impersonate hf":
                    world.Events.Add(new ImpersonateHf(properties, world));
                    break;
                case "item stolen":
                    world.Events.Add(new ItemStolen(properties, world));
                    break;
                case "new site leader":
                    world.Events.Add(new NewSiteLeader(properties, world));
                    break;
                case "peace accepted":
                    world.Events.Add(new PeaceAccepted(properties, world));
                    break;
                case "peace rejected":
                    world.Events.Add(new PeaceRejected(properties, world));
                    break;
                case "plundered site":
                    world.Events.Add(new PlunderedSite(properties, world));
                    break;
                case "reclaim site":
                    world.Events.Add(new ReclaimSite(properties, world));
                    break;
                case "remove hf entity link":
                    world.Events.Add(new RemoveHfEntityLink(properties, world));
                    break;
                case "artifact created":
                    world.Events.Add(new ArtifactCreated(properties, world));
                    break;
                case "diplomat lost":
                    world.Events.Add(new DiplomatLost(properties, world));
                    break;
                case "entity created":
                    world.Events.Add(new EntityCreated(properties, world));
                    break;
                case "hf revived":
                    world.Events.Add(new HfRevived(properties, world));
                    break;
                case "masterpiece arch design":
                    world.Events.Add(new MasterpieceArchDesign(properties, world));
                    break;
                case "masterpiece arch constructed":
                    world.Events.Add(new MasterpieceArchConstructed(properties, world));
                    break;
                case "masterpiece engraving":
                    world.Events.Add(new MasterpieceEngraving(properties, world));
                    break;
                case "masterpiece food":
                    world.Events.Add(new MasterpieceFood(properties, world));
                    break;
                case "masterpiece lost":
                    world.Events.Add(new MasterpieceLost(properties, world));
                    break;
                case "masterpiece item":
                    world.Events.Add(new MasterpieceItem(properties, world));
                    break;
                case "masterpiece item improvement":
                    world.Events.Add(new MasterpieceItemImprovement(properties, world));
                    break;
                case "merchant":
                    world.Events.Add(new Merchant(properties, world));
                    break;
                case "site abandoned":
                    world.Events.Add(new SiteAbandoned(properties, world));
                    break;
                case "site died":
                    world.Events.Add(new SiteDied(properties, world));
                    break;
                case "add hf site link":
                    world.Events.Add(new AddHfSiteLink(properties, world));
                    break;
                case "created structure":
                    world.Events.Add(new CreatedStructure(properties, world));
                    break;
                case "hf razed structure":
                    world.Events.Add(new HfRazedStructure(properties, world));
                    break;
                case "remove hf site link":
                    world.Events.Add(new RemoveHfSiteLink(properties, world));
                    break;
                case "replaced structure":
                    world.Events.Add(new ReplacedStructure(properties, world));
                    break;
                case "site taken over":
                    world.Events.Add(new SiteTakenOver(properties, world));
                    break;
                case "entity relocate":
                    world.Events.Add(new EntityRelocate(properties, world));
                    break;
                case "hf gains secret goal":
                    world.Events.Add(new HfGainsSecretGoal(properties, world));
                    break;
                case "hf profaned structure":
                    world.Events.Add(new HfProfanedStructure(properties, world));
                    break;
                case "hf does interaction":
                    world.Events.Add(new HfDoesInteraction(properties, world));
                    break;
                case "entity primary criminals":
                    world.Events.Add(new EntityPrimaryCriminals(properties, world));
                    break;
                case "hf confronted":
                    world.Events.Add(new HfConfronted(properties, world));
                    break;
                case "assume identity":
                    world.Events.Add(new AssumeIdentity(properties, world));
                    break;
                case "entity law":
                    world.Events.Add(new EntityLaw(properties, world));
                    break;
                case "change hf body state":
                    world.Events.Add(new ChangeHfBodyState(properties, world));
                    break;
                case "razed structure":
                    world.Events.Add(new RazedStructure(properties, world));
                    break;
                case "hf learns secret":
                    world.Events.Add(new HfLearnsSecret(properties, world));
                    break;
                case "artifact stored":
                    world.Events.Add(new ArtifactStored(properties, world));
                    break;
                case "artifact possessed":
                    world.Events.Add(new ArtifactPossessed(properties, world));
                    break;
                case "agreement made":
                    world.Events.Add(new AgreementMade(properties, world));
                    break;
                case "agreement rejected":
                    world.Events.Add(new AgreementRejected(properties, world));
                    break;
                case "artifact lost":
                    world.Events.Add(new ArtifactLost(properties, world));
                    break;
                case "site dispute":
                    world.Events.Add(new SiteDispute(properties, world));
                    break;
                case "hf attacked site":
                    world.Events.Add(new HfAttackedSite(properties, world));
                    break;
                case "hf destroyed site":
                    world.Events.Add(new HfDestroyedSite(properties, world));
                    break;
                case "agreement formed":
                    world.Events.Add(new AgreementFormed(properties, world));
                    break;
                case "site tribute forced":
                    world.Events.Add(new SiteTributeForced(properties, world));
                    break;
                case "insurrection started":
                    world.Events.Add(new InsurrectionStarted(properties, world));
                    break;
                case "procession":
                    world.Events.Add(new Procession(properties, world));
                    break;
                case "ceremony":
                    world.Events.Add(new Ceremony(properties, world));
                    break;
                case "performance":
                    world.Events.Add(new Performance(properties, world));
                    break;
                case "competition":
                    world.Events.Add(new Competition(properties, world));
                    break;
                case "written content composed":
                    world.Events.Add(new WrittenContentComposed(properties, world));
                    break;
                case "poetic form created":
                    world.Events.Add(new PoeticFormCreated(properties, world));
                    break;
                case "musical form created":
                    world.Events.Add(new MusicalFormCreated(properties, world));
                    break;
                case "dance form created":
                    world.Events.Add(new DanceFormCreated(properties, world));
                    break;
                case "knowledge discovered":
                    world.Events.Add(new KnowledgeDiscovered(properties, world));
                    break;
                case "hf relationship denied":
                    world.Events.Add(new HfRelationShipDenied(properties, world));
                    break;
                case "regionpop incorporated into entity":
                    world.Events.Add(new RegionpopIncorporatedIntoEntity(properties, world));
                    break;
                case "artifact destroyed":
                    world.Events.Add(new ArtifactDestroyed(properties, world));
                    break;
                case "first contact":
                    world.Events.Add(new FirstContact(properties, world));
                    break;
                case "site retired":
                    world.Events.Add(new SiteRetired(properties, world));
                    break;
                case "agreement concluded":
                    world.Events.Add(new AgreementConcluded(properties, world));
                    break;
                case "hf reach summit":
                    world.Events.Add(new HfReachSummit(properties, world));
                    break;
                case "artifact transformed":
                    world.Events.Add(new ArtifactTransformed(properties, world));
                    break;
                case "masterpiece dye":
                    world.Events.Add(new MasterpieceDye(properties, world));
                    break;
                case "hf disturbed structure":
                    world.Events.Add(new HfDisturbedStructure(properties, world));
                    break;
                case "hfs formed reputation relationship":
                    world.Events.Add(new HfsFormedReputationRelationship(properties, world));
                    break;
                case "artifact given":
                    world.Events.Add(new ArtifactGiven(properties, world));
                    break;
                case "artifact claim formed":
                    world.Events.Add(new ArtifactClaimFormed(properties, world));
                    break;
                case "hf recruited unit type for entity":
                    world.Events.Add(new HfRecruitedUnitTypeForEntity(properties, world));
                    break;
                case "hf prayed inside structure":
                    world.Events.Add(new HfPrayedInsideStructure(properties, world));
                    break;
                case "artifact copied":
                    world.Events.Add(new ArtifactCopied(properties, world));
                    break;
                case "artifact recovered":
                    world.Events.Add(new ArtifactRecovered(properties, world));
                    break;
                case "artifact found":
                    world.Events.Add(new ArtifactFound(properties, world));
                    break;
                case "hf viewed artifact":
                    world.Events.Add(new HfViewedArtifact(properties, world));
                    break;
                case "sneak into site":
                    world.Events.Add(new SneakIntoSite(properties, world));
                    break;
                case "spotted leaving site":
                    world.Events.Add(new SpottedLeavingSite(properties, world));
                    break;
                case "entity searched site":
                    world.Events.Add(new EntitySearchedSite(properties, world));
                    break;
                case "hf freed":
                    world.Events.Add(new HfFreed(properties, world));
                    break;
                case "tactical situation":
                    world.Events.Add(new TacticalSituation(properties, world));
                    break;
                case "squad vs squad":
                    world.Events.Add(new SquadVsSquad(properties, world));
                    break;
                case "agreement void":
                    world.Events.Add(new AgreementVoid(properties, world));
                    break;
                case "entity rampaged in site":
                    world.Events.Add(new EntityRampagedInSite(properties, world));
                    break;
                case "entity fled site":
                    world.Events.Add(new EntityFledSite(properties, world));
                    break;
                case "entity expels hf":
                    world.Events.Add(new EntityExpelsHf(properties, world));
                    break;
                case "site surrendered":
                    world.Events.Add(new SiteSurrendered(properties, world));
                    break;
                case "remove hf hf link":
                    world.Events.Add(new RemoveHfHfLink(properties, world));
                    break;
                case "holy city declaration":
                    world.Events.Add(new HolyCityDeclaration(properties, world));
                    break;
                case "hf performed horrible experiments":
                    world.Events.Add(new HfPerformedHorribleExperiments(properties, world));
                    break;
                case "entity incorporated":
                    world.Events.Add(new EntityIncorporated(properties, world));
                    break;
                case "gamble":
                    world.Events.Add(new Gamble(properties, world));
                    break;
                case "trade":
                    world.Events.Add(new Trade(properties, world));
                    break;
                case "hf equipment purchase":
                    world.Events.Add(new HfEquipmentPurchase(properties, world));
                    break;
                case "entity overthrown":
                    world.Events.Add(new EntityOverthrown(properties, world));
                    break;
                case "failed frame attempt":
                    world.Events.Add(new FailedFrameAttempt(properties, world));
                    break;
                case "hf convicted":
                    world.Events.Add(new HfConvicted(properties, world));
                    break;
                case "failed intrigue corruption":
                    world.Events.Add(new FailedIntrigueCorruption(properties, world));
                    break;
                case "hfs formed intrigue relationship":
                    world.Events.Add(new HfsFormedIntrigueRelationship(properties, world));
                    break;
                case "entity alliance formed":
                    world.Events.Add(new EntityAllianceFormed(properties, world));
                    break;
                case "entity dissolved":
                    world.Events.Add(new EntityDissolved(properties, world));
                    break;
                case "add hf entity honor":
                    world.Events.Add(new AddHfEntityHonor(properties, world));
                    break;
                case "entity breach feature layer":
                    world.Events.Add(new EntityBreachFeatureLayer(properties, world));
                    break;
                case "entity equipment purchase":
                    world.Events.Add(new EntityEquipmentPurchase(properties, world));
                    break;
                case "hf ransomed":
                    world.Events.Add(new HfRansomed(properties, world));
                    break;
                case "hf preach":
                    world.Events.Add(new HfPreach(properties, world));
                    break;
                case "modified building":
                    world.Events.Add(new ModifiedBuilding(properties, world));
                    break;
                case "hf interrogated":
                    world.Events.Add(new HfInterrogated(properties, world));
                    break;
                case "entity persecuted":
                    world.Events.Add(new EntityPersecuted(properties, world));
                    break;
                case "building profile acquired":
                    world.Events.Add(new BuildingProfileAcquired(properties, world));
                    break;
                case "hf enslaved":
                    world.Events.Add(new HfEnslaved(properties, world));
                    break;
                case "hf asked about artifact":
                    world.Events.Add(new HfAskedAboutArtifact(properties, world));
                    break;
                case "hf carouse":
                    world.Events.Add(new HfCarouse(properties, world));
                    break;
                case "sabotage":
                    world.Events.Add(new Sabotage(properties, world));
                    break;
                default:
                    world.ParsingErrors.Report("\nUnknown Event: " + type);
                    break;
            }
        }

        internal static void AddEventCollectionToWorld(string type, World world, List<Property> properties)
        {
            switch (type)
            {
                case "abduction":
                    world.EventCollections.Add(new Abduction(properties, world));
                    break;
                case "battle":
                    world.EventCollections.Add(new Battle(properties, world));
                    break;
                case "beast attack":
                    world.EventCollections.Add(new BeastAttack(properties, world));
                    break;
                case "duel":
                    world.EventCollections.Add(new Duel(properties, world));
                    break;
                case "journey":
                    world.EventCollections.Add(new Journey(properties, world));
                    break;
                case "site conquered":
                    world.EventCollections.Add(new SiteConquered(properties, world));
                    break;
                case "theft":
                    world.EventCollections.Add(new Theft(properties, world));
                    break;
                case "war":
                    world.EventCollections.Add(new War(properties, world));
                    break;
                case "insurrection":
                    world.EventCollections.Add(new Insurrection(properties, world));
                    break;
                case "occasion":
                    world.EventCollections.Add(new Occasion(properties, world));
                    break;
                case "procession":
                    world.EventCollections.Add(new ProcessionCollection(properties, world));
                    break;
                case "ceremony":
                    world.EventCollections.Add(new CeremonyCollection(properties, world));
                    break;
                case "performance":
                    world.EventCollections.Add(new PerformanceCollection(properties, world));
                    break;
                case "competition":
                    world.EventCollections.Add(new CompetitionCollection(properties, world));
                    break;
                case "purge":
                    world.EventCollections.Add(new Purge(properties, world));
                    break;
                case "raid":
                    world.EventCollections.Add(new Raid(properties, world));
                    break;
                case "persecution":
                    world.EventCollections.Add(new Persecution(properties, world));
                    break;
                case "entity overthrown":
                    world.EventCollections.Add(new EntityOverthrownCollection(properties, world));
                    break;
                default:
                    world.ParsingErrors.Report("\nUnknown Event Collection: " + type);
                    break;
            }
        }
    }
}