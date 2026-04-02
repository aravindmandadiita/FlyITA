using FlyITA.Core.Models;

namespace FlyITA.Core.Interfaces;

public interface IGuestProfileView
{
    // General Info
    string LegalFirstName { get; set; }
    string LegalMiddleName { get; set; }
    string LegalLastName { get; set; }
    string Prefix { get; set; }
    string Suffix { get; set; }
    string Nickname { get; set; }
    string BadgeName { get; set; }
    string DateOfBirth { get; set; }
    string Gender { get; set; }
    string Country { get; set; }
    string Citizenship { get; set; }

    // Dropdown options
    List<KeyValuePair<string, string>> PrefixOptions { get; set; }
    List<KeyValuePair<string, string>> SuffixOptions { get; set; }
    List<KeyValuePair<string, string>> GenderOptions { get; set; }
    List<KeyValuePair<string, string>> CountryOptions { get; set; }
    List<KeyValuePair<string, string>> CitizenshipOptions { get; set; }

    // Passport
    string PassportFirstName { get; set; }
    string PassportMiddleName { get; set; }
    string PassportLastName { get; set; }
    string PassportNumber { get; set; }
    string PassportExpirationDate { get; set; }
    string PassportIssueDate { get; set; }
    string PassportIssuingAuthority { get; set; }
    string PassportNationality { get; set; }
    string PassportIssuingCountry { get; set; }

    // Birth Certificate
    string BirthCertificateNumber { get; set; }
    string ProofOfCitizenshipType { get; set; }

    // Special Requests
    string DietaryPreference { get; set; }
    string PhysicalAssistance { get; set; }

    // Section visibility
    bool GeneralInfoSectionVisible { get; set; }
    bool PersonalInfoSectionVisible { get; set; }
    bool ProofOfCitizenshipVisible { get; set; }
    bool SpecialRequestVisible { get; set; }
    bool PassportSectionVisible { get; set; }

    // Labels
    string WelcomeText { get; set; }
    string InformationText { get; set; }

    // Custom fields
    CustomFieldControlCollection CustomFieldControls { get; set; }
}
