using PsychAppointments_API.Models;
using PsychAppointments_API.Service;

namespace PsychAppointmentsTests;

public class SlotServiceTest
{
    public bool Overlap(SlotDTO slot1, SlotDTO slot2)
    {
        if (slot1.Date != slot2.Date)
        {
            return false;
        }

        List<SlotDTO> slots = new List<SlotDTO>(){ slot1, slot2 };  
        slots.Sort((sl1, sl2) => sl1.SlotStart.CompareTo(sl2.SlotStart));
        
        if (slots[0].SlotEnd > slots[1].SlotStart)
        {
            return true;
        }

        return false;
    }

    [Test]
    public void SlotsDoNotOverlapTest()
    {
        Psychologist psychologist = new Psychologist();
        Location location = new Location();
        DateTime date = new DateTime(2023, 09, 19);
        DateTime start1 = new DateTime(2023, 09, 19, 10, 00, 00);
        DateTime end1 = new DateTime(2023, 09, 19, 11, 00, 00);
        SlotDTO slot1 = new SlotDTO(psychologist, location, date, start1, end1);
        
        DateTime start2 = new DateTime(2023, 09, 19, 12, 00, 00);
        DateTime end2 = new DateTime(2023, 09, 19, 13, 00, 00);
        SlotDTO slot2 = new SlotDTO(psychologist, location, date, start2, end2);

        bool result = Overlap(slot1, slot2);
        
        Assert.That(result, Is.EqualTo(false));
    }

    [Test]
    public void SlotsDoNotOverlapOneEndsWhenOtherStartsTest()
    {
        Psychologist psychologist = new Psychologist();
        Location location = new Location();
        DateTime date = new DateTime(2023, 09, 19);
        DateTime start1 = new DateTime(2023, 09, 19, 10, 00, 00);
        DateTime end1 = new DateTime(2023, 09, 19, 11, 00, 00);
        SlotDTO slot1 = new SlotDTO(psychologist, location, date, start1, end1);
        
        DateTime start2 = new DateTime(2023, 09, 19, 11, 00, 00);
        DateTime end2 = new DateTime(2023, 09, 19, 12, 00, 00);
        SlotDTO slot2 = new SlotDTO(psychologist, location, date, start2, end2);

        bool result = Overlap(slot1, slot2);
        
        Assert.That(result, Is.EqualTo(false));
    }

    [Test]
    public void SlotsOverlapTest()
    {
        Psychologist psychologist = new Psychologist();
        Location location = new Location();
        DateTime date = new DateTime(2023, 09, 19);
        DateTime start1 = new DateTime(2023, 09, 19, 10, 00, 00);
        DateTime end1 = new DateTime(2023, 09, 19, 11, 00, 00);
        SlotDTO slot1 = new SlotDTO(psychologist, location, date, start1, end1);
        
        DateTime start2 = new DateTime(2023, 09, 19, 10, 45, 00);
        DateTime end2 = new DateTime(2023, 09, 19, 13, 00, 00);
        SlotDTO slot2 = new SlotDTO(psychologist, location, date, start2, end2);

        bool result = Overlap(slot1, slot2);
        
        Assert.That(result, Is.EqualTo(true));
    }

    [Test]
    public void DifferentDateNoOverlapTest()
    {
        Psychologist psychologist = new Psychologist();
        Location location = new Location();
        DateTime date = new DateTime(2023, 09, 19);
        DateTime start1 = new DateTime(2023, 09, 19, 10, 00, 00);
        DateTime end1 = new DateTime(2023, 09, 19, 11, 00, 00);
        SlotDTO slot1 = new SlotDTO(psychologist, location, date, start1, end1);

        DateTime date2 = new DateTime(2023, 09, 20);
        DateTime start2 = new DateTime(2023, 09, 19, 12, 00, 00);
        DateTime end2 = new DateTime(2023, 09, 19, 13, 00, 00);
        SlotDTO slot2 = new SlotDTO(psychologist, location, date2, start2, end2);

        bool result = Overlap(slot1, slot2);
        
        Assert.That(result, Is.EqualTo(false));
    }

    [Test]
    public void DifferentDateOverlapTest()
    {
        Psychologist psychologist = new Psychologist();
        Location location = new Location();
        DateTime date = new DateTime(2023, 09, 19);
        DateTime start1 = new DateTime(2023, 09, 19, 10, 00, 00);
        DateTime end1 = new DateTime(2023, 09, 19, 11, 00, 00);
        SlotDTO slot1 = new SlotDTO(psychologist, location, date, start1, end1);
        
        DateTime date2 = new DateTime(2023, 09, 20);
        DateTime start2 = new DateTime(2023, 09, 19, 10, 45, 00);
        DateTime end2 = new DateTime(2023, 09, 19, 13, 00, 00);
        SlotDTO slot2 = new SlotDTO(psychologist, location, date2, start2, end2);

        bool result = Overlap(slot1, slot2);
        
        Assert.That(result, Is.EqualTo(false));
    }
}