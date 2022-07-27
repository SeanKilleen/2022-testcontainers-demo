Feature: People Working at Excella

A short summary of the feature

@tag1
Scenario: Seeing people who work at Excella
	Given I have added Vince Smith as an employee who works at Excella
		And I have added Ranil Arieta as an employee who works at Excella
		And I have added Sean Killeen as an employee who works at Excella
		And I have added Christine Kuder as an employee who works at Excella
		And I have added Robert Fahl as an employee who works at Excella
		When I get the employees who work at Excella
	Then the employee list should contain 5 people
