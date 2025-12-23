// This array contains all 8 Divisions and all 64 Districts
const ALL_BANGLADESH_DIVISIONS = [
    { name: "Dhaka", districts: ["Dhaka", "Faridpur", "Gazipur", "Gopalganj", "Kishoreganj", "Madaripur", "Manikganj", "Munshiganj", "Narayanganj", "Narsingdi", "Rajbari", "Shariatpur", "Tangail"] },
    { name: "Chittagong", districts: ["Bandarban", "Brahmanbaria", "Chandpur", "Chittagong", "Comilla", "Cox's Bazar", "Feni", "Khagrachhari", "Lakshmipur", "Noakhali", "Rangamati"] },
    { name: "Rajshahi", districts: ["Bogra", "Joypurhat", "Naogaon", "Natore", "Nawabganj", "Pabna", "Rajshahi", "Sirajganj"] },
    { name: "Khulna", districts: ["Bagerhat", "Chuadanga", "Jessore", "Jhenaidah", "Khulna", "Kushtia", "Magura", "Meherpur", "Narail", "Satkhira"] },
    { name: "Barisal", districts: ["Barguna", "Barisal", "Bhola", "Jhalokati", "Patuakhali", "Pirojpur"] },
    { name: "Sylhet", districts: ["Habiganj", "Moulvibazar", "Sunamganj", "Sylhet"] },
    { name: "Rangpur", districts: ["Dinajpur", "Gaibandha", "Kurigram", "Lalmonirhat", "Nilphamari", "Panchagarh", "Rangpur", "Thakurgaon"] },
    { name: "Mymensingh", districts: ["Jamalpur", "Mymensingh", "Netrokona", "Sherpur"] }
];

// Areas eligible for Inside Dhaka (Lower Rate) delivery
const INSIDE_DHAKA_AREAS = ["Dhaka", "Gazipur", "Narayanganj", "Savar", "Keraniganj"];

// Helper function to easily look up districts by division name
const DISTRICTS_BY_DIVISION = ALL_BANGLADESH_DIVISIONS.reduce((acc, div) => {
    acc[div.name] = div.districts;
    return acc;
}, {});