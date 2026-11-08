using System.ComponentModel.DataAnnotations;
namespace RaceDay.Mvc.Models;
public class RaceEvent {
 public int Id {get;set;} [Required] public string Name {get;set;}="";
 public string? Description {get;set;} [Required] public DateOnly EventDate {get;set;}=DateOnly.FromDateTime(DateTime.Today.AddDays(30));
 public TimeOnly? StartTime {get;set;} [Required] public string Location {get;set;}="";
 [Range(0.01,9999)] public decimal Distance {get;set;} [Range(0,999999)] public decimal? EntryFee {get;set;}
 [Range(1,int.MaxValue)] public int EventTypeId {get;set;}=1;
 public int OrganizerId {get;set;} public DateOnly? RegistrationDeadline {get;set;}
 public int? MaxParticipants {get;set;}=100; public string Status {get;set;}="Open";
}
public class CategoryModel {public int Id {get;set;} [Required] public string Name {get;set;}=""; public string? Description {get;set;}}
public class EnrolmentModel {public int Id {get;set;} public int EventId {get;set;} public int UserId {get;set;} public int CategoryId {get;set;} public string Status {get;set;}=""; public string? RaceNumber {get;set;}}
public class ResultInput { [Range(1,int.MaxValue)] public int EnrollmentId {get;set;} [Required] public TimeSpan? FinishTime {get;set;} [Range(1,int.MaxValue)] public int? Position {get;set;} public decimal? Pace {get;set;} public string? Status {get;set;} }
public class RegistrationInput { [Required] public string Name {get;set;}=""; [Required] public string Surname {get;set;}=""; [EmailAddress,Required] public string Email {get;set;}=""; [Required,MinLength(6)] public string Password {get;set;}=""; [Required] public string Role {get;set;}="Participant"; }
