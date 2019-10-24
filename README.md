# Loan Quotes MOM :moneybag:

This repository implements a **MOM** (**M**essage-**o**riented **m**iddleware) :incoming_envelope: example for a loan request application. Its purpose is to be used by a customer :bust_in_silhouette:, who needs an urgent bank loan :money_with_wings:.

</br>

---
## Functionality :hammer_and_wrench:
* Our bank application :bank: will be instantiated with a new **id** everytime a new instance is fired :fire:. This way it makes it possible to run several concurrent bank applications to send :postbox: loan proposals. 
* The customer application:
  * makes a loan request :pencil:;
  * collects :inbox_tray: non-binding bank quotes from (simulated) bank applications;
  * compares the quotes :balance_scale: and selects one, based on its own financial status and criteria.

</br>

---
## Set up  :control_knobs:

NB:bangbang: **Before** running our 2 projects, make sure you have _RabbitMQ_ running :rabbit:.  

If you have _Docker_ within reach :whale: you can simply run the following command to start it:
<p align="center"><code>docker run --name rabbitMQ -p 15672:15672 -p 5672:5672 rabbitmq:management</code></p>  

> Optional: To make sure _RabbitMQ_ is running, navigate to `localhost:15672`in your browser

After you have cloned down :arrow_down: this repository, you need to assure you have installed **RabbitMQ.Client** nuget package for both (**Client** and **Producer**) projects.

Change the IP addresses in the code to the one your RabbitMQ instance is running at (normally it's _localhost_) - [ln89](https://github.com/elit0451/LoanQuotesMOM/blob/699f2f0deb27c82bd109f1870d69bfc81baabdf5/LoanQuotesMOM/Program.cs#L89) and [ln106](https://github.com/elit0451/LoanQuotesMOM/blob/699f2f0deb27c82bd109f1870d69bfc81baabdf5/LoanQuotesMOM/Program.cs#L106) on the Client project and [ln31](https://github.com/elit0451/LoanQuotesMOM/blob/699f2f0deb27c82bd109f1870d69bfc81baabdf5/LoanQuotesMOMProducer/Program.cs#L31) and [ln69](https://github.com/elit0451/LoanQuotesMOM/blob/699f2f0deb27c82bd109f1870d69bfc81baabdf5/LoanQuotesMOMProducer/Program.cs#L69) on Producer.

Run several instances of both and follow the on-screen instructions :calling: 

</br>

___
> #### Assignment made by:   
`David Alves 👨🏻‍💻 ` :octocat: [Github](https://github.com/davi7725) <br />
`Elitsa Marinovska 👩🏻‍💻 ` :octocat: [Github](https://github.com/elit0451) <br />
> Attending "System Integration" course of Software Development bachelor's degree
