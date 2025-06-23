.PHONY: ai
ai:
	cat doc/*.md *.fsproj lib/*.fs > tmp/$(APP).AI.md
