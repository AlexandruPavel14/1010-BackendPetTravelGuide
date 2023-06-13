import csv
import random
import string

# Define the name of the output file
output_file = "generated_users.csv"

# Define the number of rows
num_rows = 11555

# Generate a random string of lowercase letters for the names
def generate_random_name(length):
    letters = string.ascii_lowercase
    return ''.join(random.choice(letters) for _ in range(length))

# Open the output file
with open(output_file, "w", newline='') as output_csv:
    # Create a CSV writer object
    csv_writer = csv.writer(output_csv)

    # Write the header row to the output CSV file
    csv_writer.writerow(["ID", "First Name", "Last Name"])

    # Iterate from 1 to the desired number of rows
    for id_count in range(1, num_rows+1):
        # Generate random names
        first_name = generate_random_name(5)
        last_name = generate_random_name(7)

        # Write the row to the output CSV file
        csv_writer.writerow([id_count, first_name, last_name])
