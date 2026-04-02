namespace FlyITA.Core.Models;

public class GuestProfileModel
{
    // General Info
    public string LegalFirstName { get; set; } = string.Empty;
    public string LegalMiddleName { get; set; } = string.Empty;
    public string LegalLastName { get; set; } = string.Empty;
    public string Prefix { get; set; } = string.Empty;
    public string Suffix { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string BadgeName { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Citizenship { get; set; } = string.Empty;

    // Dropdown options
    public List<KeyValuePair<string, string>> PrefixOptions { get; set; } = new();
    public List<KeyValuePair<string, string>> SuffixOptions { get; set; } = new();
    public List<KeyValuePair<string, string>> GenderOptions { get; set; } = new();
    public List<KeyValuePair<string, string>> CountryOptions { get; set; } = new();
    public List<KeyValuePair<string, string>> CitizenshipOptions { get; set; } = new();

    // Passport
    public string PassportFirstName { get; set; } = string.Empty;
    public string PassportMiddleName { get; set; } = string.Empty;
    public string PassportLastName { get; set; } = string.Empty;
    public string PassportNumber { get; set; } = string.Empty;
    public string PassportExpirationDate { get; set; } = string.Empty;
    public string PassportIssueDate { get; set; } = string.Empty;
    public string PassportIssuingAuthority { get; set; } = string.Empty;
    public string PassportNationality { get; set; } = string.Empty;
    public string PassportIssuingCountry { get; set; } = string.Empty;
    public List<KeyValuePair<string, string>> PassportIssuingCountryOptions { get; set; } = new();

    // Birth Certificate
    public string BirthCertificateNumber { get; set; } = string.Empty;
    public string BirthCertificateIssueDate { get; set; } = string.Empty;
    public string BirthCertificateIssuingCountry { get; set; } = string.Empty;
    public string ProofOfCitizenshipType { get; set; } = string.Empty;

    // Special Requests
    public string DietaryPreference { get; set; } = string.Empty;
    public string PhysicalAssistance { get; set; } = string.Empty;

    // Section visibility
    public bool GeneralInfoSectionVisible { get; set; } = true;
    public bool PersonalInfoSectionVisible { get; set; } = true;
    public bool ProofOfCitizenshipVisible { get; set; }
    public bool SpecialRequestVisible { get; set; }
    public bool PassportSectionVisible { get; set; }
    public bool BirthCertificateSectionVisible { get; set; }
    public bool NameSectionVisible { get; set; } = true;
    public bool BadgeNameSectionVisible { get; set; }
    public bool NicknameSectionVisible { get; set; }

    // Field visibility
    public bool PrefixVisible { get; set; } = true;
    public bool SuffixVisible { get; set; } = true;
    public bool MiddleNameVisible { get; set; } = true;
    public bool DateOfBirthVisible { get; set; } = true;
    public bool GenderVisible { get; set; } = true;

    // Required flags
    public bool PrefixRequired { get; set; }
    public bool SuffixRequired { get; set; }
    public bool DateOfBirthRequired { get; set; }
    public bool GenderRequired { get; set; }
    public bool PassportRequired { get; set; }

    // Labels / text
    public string WelcomeText { get; set; } = string.Empty;
    public string InformationText { get; set; } = string.Empty;
    public string GeneralInfoSectionHeading { get; set; } = string.Empty;
    public string PersonalInfoSectionHeading { get; set; } = string.Empty;
    public string ProofOfCitizenshipHeading { get; set; } = string.Empty;
    public string SpecialRequestHeading { get; set; } = string.Empty;
    public string PassportSectionHeading { get; set; } = string.Empty;

    // Custom fields
    public CustomFieldControlCollection CustomFieldControls { get; set; } = new();
}
