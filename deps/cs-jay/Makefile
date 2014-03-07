thisdir := jay
SUBDIRS := 
#include ../build/rules.make

prefix=/usr
LOCAL_CFLAGS = -DSKEL_DIRECTORY=\""$(prefix)/share/jay"\"
CCOMPILE = @gcc -Wall -O2 $(LOCAL_CFLAGS) -Wno-return-type -Wno-implicit -Wno-parentheses -Wno-unused -Wno-uninitialized

sources = closure.c error.c lalr.c lr0.c main.c mkpar.c output.c reader.c \
          symtab.c verbose.c warshall.c

objects = $(sources:.c=.o)

datafiles = ACKNOWLEDGEMENTS NEW_FEATURES NOTES README README.jay skeleton \
            skeleton.cs

DISTFILES = $(datafiles) $(sources) jay.1 $(wildcard *.h)

all-local: jay

install-local: jay
uninstall-local:


ifndef NO_INSTALL
install-local:
	$(MKINSTALLDIRS) $(DESTDIR)$(prefix)/bin
	$(MKINSTALLDIRS) $(DESTDIR)$(prefix)/share/jay
	$(MKINSTALLDIRS) $(DESTDIR)$(prefix)/share/man/man1
	$(INSTALL_BIN) jay $(DESTDIR)$(prefix)/bin
	for datafile in $(datafiles) ; do \
	   $(INSTALL_DATA) $$datafile $(DESTDIR)$(prefix)/share/jay ; \
	done
	$(INSTALL_DATA) jay.1 $(DESTDIR)$(prefix)/share/man/man1

uninstall-local:
	-rm -f $(DESTDIR)$(prefix)/bin/jay
	for datafile in $(datafiles) ; do \
	   rm -f $(DESTDIR)$(prefix)/share/jay/$$datafile || : ; \
	done
	-rm -f $(DESTDIR)$(prefix)/share/man/man1/jay.1
endif

clean clean-local:
	rm -f jay *.o *.exe *.pdb

dist-local: dist-default

test-local run-test-local run-test-ondotnet-local doc-update-local:

jay: $(objects)
	$(CCOMPILE) -o $@ $^

%.o:%.c
	$(CCOMPILE) -c -o $@ $^


